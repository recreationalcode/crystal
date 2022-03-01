using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipCollider : MonoBehaviour
{
    public Ship ship;

    void OnParticleCollision(GameObject other)
    {
        ParticleSystem p = other.GetComponent<ParticleSystem>();
        List<ParticleCollisionEvent> pec = new List<ParticleCollisionEvent>(
            ParticlePhysicsExtensions.GetSafeCollisionEventSize(p)
        );
        int numCollisionEvents = p.GetCollisionEvents(gameObject, pec);

        if (
            (ship.CompareTag("Fractal") && other.gameObject.CompareTag("Shadow")) ||
            (ship.CompareTag("Shadow") && other.gameObject.CompareTag("Fractal")))
        {
            Ship otherShip = Ship.GetShipReference(other);

            if (otherShip != null)
            {
                ship.Hit(otherShip.GetDamage() * numCollisionEvents, -pec[0].normal);
            }
            else
            {
                // TODO Handle tower case
                ship.Hit(numCollisionEvents, -pec[0].normal);
            }
        }
    }
}
