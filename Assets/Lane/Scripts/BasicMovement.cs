using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class BasicMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 8f;
    public float acceleration = 20f;
    public float deceleration = 25f;

    [Header("Air Movement")]
    public float airAcceleration = 10f;
    public float airDeceleration = 5f;

    [Header("Jumping")]
    public float jumpHeight = 3f;
    public float airJumpHeight = 2f;
    public float gravity = -25f;

    [Header("Shared State")]
    public int jumpCount = 0;
    public int maxJumps = 2;

    [HideInInspector] public bool isDashing = false;
    [HideInInspector] public bool hasAirDashed = false;
    [HideInInspector] public bool isSliding = false;
    [HideInInspector] public bool isFloating = false;
    [HideInInspector] public bool preserveDashMomentum = false;

    [HideInInspector] public float groundedBuffer = 0f;
    [HideInInspector] public Vector3 moveVelocity;
    [HideInInspector] public Vector3 verticalVelocity;

    private CharacterController controller;


    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        float inputX = Input.GetAxis("Horizontal");
        float inputZ = Input.GetAxis("Vertical");

        Vector3 inputDir = new Vector3(inputX, 0f, inputZ);
        inputDir = CameraRelative(inputDir);
        inputDir = Vector3.ClampMagnitude(inputDir, 1f);

        if (controller.isGrounded)
            groundedBuffer = 0.1f;
        else
            groundedBuffer -= Time.deltaTime;

        bool isGrounded = groundedBuffer > 0f;

        float currentAccel = isGrounded ? acceleration : airAcceleration;
        float currentDecel = isGrounded ? deceleration : airDeceleration;

        if (preserveDashMomentum && !isGrounded)
        {
            if (inputDir.magnitude > 0.1f)
            {
                float dot = Vector3.Dot(moveVelocity.normalized, inputDir.normalized);
                if (dot < 0.2f)
                    preserveDashMomentum = false;
            }

            if (!preserveDashMomentum)
                ApplyAirMovement(inputDir, currentAccel, currentDecel);
        }
        else
        {
            ApplyAirMovement(inputDir, currentAccel, currentDecel);
        }

        HandleJumpAndGravity(isGrounded);

        Vector3 finalMove = moveVelocity + verticalVelocity;
        controller.Move(finalMove * Time.deltaTime);

        if (!isDashing && !isSliding && moveVelocity.magnitude > 0.1f)
        {
            Quaternion targetRot = Quaternion.LookRotation(moveVelocity);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, 8f * Time.deltaTime);
        }
    }

    private void HandleJumpAndGravity(bool isGrounded)
    {
        if (isGrounded)
        {
            if (verticalVelocity.y < 0)
                verticalVelocity.y = -2f;

            jumpCount = 0;
            hasAirDashed = false;
            preserveDashMomentum = false;
            isFloating = false;

            // Ground jump ONLY
            if (!isDashing && !isSliding && Input.GetButtonDown("Jump"))
            {
                verticalVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
                jumpCount = 1; // first jump
            }
        }
        else
        {
            // Gravity only — DoubleJump handles the air jump
            verticalVelocity.y += gravity * Time.deltaTime;
        }
    }


    public void ApplyAirMovement(Vector3 inputDir, float accel, float decel)
    {
        if (inputDir.magnitude > 0.1f)
        {
            moveVelocity = Vector3.MoveTowards(
                moveVelocity,
                inputDir * moveSpeed,
                accel * Time.deltaTime
            );
        }
        else
        {
            moveVelocity = Vector3.MoveTowards(
                moveVelocity,
                Vector3.zero,
                decel * Time.deltaTime
            );
        }
    }

    public Vector3 GetCameraRelativeInput()
    {
        float inputX = Input.GetAxis("Horizontal");
        float inputZ = Input.GetAxis("Vertical");

        Vector3 inputDir = new Vector3(inputX, 0f, inputZ);
        inputDir = CameraRelative(inputDir);
        return Vector3.ClampMagnitude(inputDir, 1f);
    }

    private Vector3 CameraRelative(Vector3 input)
    {
        if (Camera.main == null) return input;

        Vector3 forward = Camera.main.transform.forward;
        Vector3 right = Camera.main.transform.right;

        forward.y = 0;
        right.y = 0;

        forward.Normalize();
        right.Normalize();

        return forward * input.z + right * input.x;
    }
}
