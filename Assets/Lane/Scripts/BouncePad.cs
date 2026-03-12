using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem.XR;


public class BouncePad : MonoBehaviour

{

    [Header("BouncePad.Settings")]
    public float bounceForce = 10f;

    [Header("Light.Settings")]
    public Light bounceLight; //lyset som skal aktiveres 
    public float lightDuration = 0.2f; //hvor lenge lyset skal være på 


    private void Start()
    {
        if (bounceLight != null)
            bounceLight.enabled = false; // sørger for at lyset starter 
    }

    private void OnTriggerEnter(Collider other)
    {

        BasicMovement movement = other.GetComponent<BasicMovement>();        if (movement != null)
        {
            Debug.Log("BouncePad: Found BasicMovement on player!");

            movement.verticalVelocity.y = bounceForce;
            movement.jumpCount = 1;

            if (bounceLight != null)
                StartCoroutine(LightFlash());
        }
        else
        {
            Debug.Log("BouncePad: BasicMovement NOT found on object!");
        }
    }


    private System.Collections.IEnumerator LightFlash()
    {

bounceLight.enabled = true;
        yield return new WaitForSeconds(lightDuration);
        bounceLight.enabled = false ;
    }

}