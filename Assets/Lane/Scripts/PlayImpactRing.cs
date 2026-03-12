using UnityEngine;

public class PlayImpactRing : MonoBehaviour
{
    public ParticleSystem impactRing;

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<BasicMovement>() != null)
        {
            impactRing.Play();
        }
    }
}