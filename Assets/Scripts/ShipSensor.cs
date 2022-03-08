using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipSensor : Sensor
{
    public Ship ship;

    private void OnTriggerEnter(Collider other)
    {
        if (ship is ShadowShip)
        {
            ShadowShip shadowShip = ship as ShadowShip;

            if (IsEnemy(ship.gameObject, other.gameObject))
            {
                shadowShip.AddTarget(other.transform.parent);
            }
        }

        if (IsFriend(ship.gameObject, other.gameObject))
        {
            Ship otherShip = Ship.GetShipReference(other);

            if (otherShip != null)
            {
                otherShip.Boost(ship);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (ship is ShadowShip)
        {
            ShadowShip shadowShip = ship as ShadowShip;

            if (IsEnemy(ship.gameObject, other.gameObject))
            {
                shadowShip.RemoveTarget(other.transform.parent);
            }
        }

        if (IsFriend(ship.gameObject, other.gameObject))
        {
            // If another Fractal ship
            Ship otherShip = Ship.GetShipReference(other);;

            if (otherShip != null)
            {
                otherShip.Hinder(ship);
            }
        }

    }
}
