using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FractalBaseCollider : MonoBehaviour
{
    public FractalBase fractalBase;

    void OnParticleCollision(GameObject other)
    {
        ParticleSystem p = other.GetComponent<ParticleSystem>();
        List<ParticleCollisionEvent> pec = new List<ParticleCollisionEvent>(
            ParticlePhysicsExtensions.GetSafeCollisionEventSize(p)
        );
        int numCollisionEvents = p.GetCollisionEvents(gameObject, pec);

        if (other.gameObject.CompareTag("Shadow"))
        {
            fractalBase.Hit(numCollisionEvents, -pec[0].normal);        
        }
    }
}
