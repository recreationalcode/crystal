using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerCollider : Sensor
{
    public Tower tower;

    void OnParticleCollision(GameObject other)
    {
        int numCollisionEvents;
        Vector3 direction;

        GetCollisionInfo(other, out numCollisionEvents, out direction);

        if (IsEnemy(tower.gameObject, other.gameObject))
        {
            Ship otherShip = Ship.GetShipReference(other);

            if (otherShip != null)
            {
                tower.Hit(otherShip.GetDamage() * numCollisionEvents, direction);
            }
        }
    }
}
