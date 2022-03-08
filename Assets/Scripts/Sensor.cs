using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sensor : MonoBehaviour
{
    public static bool IsEnemy(GameObject mine, GameObject other)
    {
        return mine.CompareTag("Fractal") && other.CompareTag("Shadow") ||
            mine.CompareTag("Shadow") && other.CompareTag("Fractal");
    }

    public static bool IsFriend(GameObject mine, GameObject other)
    {
        return mine.CompareTag("Fractal") && other.CompareTag("Fractal") ||
            mine.CompareTag("Shadow") && other.CompareTag("Shadow");
    }

    public List<ParticleCollisionEvent> GetCollisionInfo(GameObject other, out int numCollisionEvents, out Vector3 direction)
    {
        ParticleSystem p = other.GetComponent<ParticleSystem>();
        List<ParticleCollisionEvent> pec = new List<ParticleCollisionEvent>(
            ParticlePhysicsExtensions.GetSafeCollisionEventSize(p)
        );
        
        numCollisionEvents = p.GetCollisionEvents(gameObject, pec);
        direction = -pec[0].normal;

        return pec;
    }
}
