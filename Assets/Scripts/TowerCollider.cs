using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerCollider : MonoBehaviour
{
    public Tower tower;

    void OnParticleCollision(GameObject other)
    {
        ParticleSystem p = other.GetComponent<ParticleSystem>();
        List<ParticleCollisionEvent> pec = new List<ParticleCollisionEvent>(
            ParticlePhysicsExtensions.GetSafeCollisionEventSize(p)
        );
        int numCollisionEvents = p.GetCollisionEvents(gameObject, pec);

        if (other.gameObject.CompareTag("Shadow"))
        {
            Ship otherShip = Ship.GetShipReference(other);

            if (otherShip != null)
            {
                tower.Hit(otherShip.GetDamage() * numCollisionEvents, -pec[0].normal);
            }
        }
    }
}
