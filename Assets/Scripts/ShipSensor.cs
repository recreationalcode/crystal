using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipSensor : MonoBehaviour
{
    public Ship ship;

    private void OnTriggerEnter(Collider other)
    {
        if (ship is ShadowShip)
        {
            ShadowShip shadowShip = ship as ShadowShip;

            if (shadowShip.gameObject.CompareTag("Shadow") && other.gameObject.CompareTag("Fractal") ||
                shadowShip.gameObject.CompareTag("Fractal") && other.gameObject.CompareTag("Shadow"))
            {
                shadowShip.AddTarget(other.transform.parent);
            }
        }

        if ((ship is Player || ship is PlayerAI) && other.gameObject.CompareTag("Fractal"))
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

            if (shadowShip.gameObject.CompareTag("Shadow") && other.gameObject.CompareTag("Fractal") ||
                shadowShip.gameObject.CompareTag("Fractal") && other.gameObject.CompareTag("Shadow"))
            {
                shadowShip.RemoveTarget(other.transform.parent);
            }
        }

        if ((ship is Player || ship is PlayerAI) && other.gameObject.CompareTag("Fractal"))
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
