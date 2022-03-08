using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerSensor : Sensor
{
    public Tower tower;

    private void OnTriggerEnter(Collider other)
    {
        if (IsEnemy(tower.gameObject, other.gameObject))
        {
            tower.AddTarget(other.transform.parent);
        }

        if (IsFriend(tower.gameObject, other.gameObject))
        {
            Ship otherShip = Ship.GetShipReference(other);

            if (otherShip != null)
            {
                otherShip.Boost(tower);
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (IsEnemy(tower.gameObject, other.gameObject))
        {
            tower.AddTarget(other.transform.parent);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (IsEnemy(tower.gameObject, other.gameObject))
        {
            tower.RemoveTarget(other.transform.parent);
        }

        if (IsFriend(tower.gameObject, other.gameObject))
        {
            Ship otherShip = Ship.GetShipReference(other);

            if (otherShip != null)
            {
                otherShip.Hinder(tower);
            }
        }
    }
}
