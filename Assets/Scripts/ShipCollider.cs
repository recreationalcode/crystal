using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipCollider : Sensor
{
    public Ship ship;

    void OnParticleCollision(GameObject other)
    {
        int numCollisionEvents;
        Vector3 direction;

        GetCollisionInfo(other, out numCollisionEvents, out direction);

        if (IsEnemy(ship.gameObject, other.gameObject))
        {
            Ship otherShip = Ship.GetShipReference(other);

            if (otherShip != null)
            {
                ship.Hit(otherShip.GetDamage() * numCollisionEvents, direction);
            }
            else
            {
                // TODO Handle tower variable damage case
                ship.Hit(numCollisionEvents, direction);
            }
        }
    }
}
