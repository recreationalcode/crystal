using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerSensor : MonoBehaviour
{
    public Tower tower;

    private void OnTriggerEnter(Collider other)
    {
        if (tower.gameObject.CompareTag("Shadow") && other.gameObject.CompareTag("Fractal") ||
            tower.gameObject.CompareTag("Fractal") && other.gameObject.CompareTag("Shadow"))
        {
            tower.AddTarget(other.transform.parent);
        }

        if (tower.gameObject.CompareTag("Fractal") && other.gameObject.CompareTag("Fractal"))
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
        if (tower.gameObject.CompareTag("Shadow") && other.gameObject.CompareTag("Fractal") ||
            tower.gameObject.CompareTag("Fractal") && other.gameObject.CompareTag("Shadow"))
        {
            tower.AddTarget(other.transform.parent);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (tower.gameObject.CompareTag("Shadow") && other.gameObject.CompareTag("Fractal") ||
            tower.gameObject.CompareTag("Fractal") && other.gameObject.CompareTag("Shadow"))
        {
            tower.RemoveTarget(other.transform.parent);
        }

        if (tower.gameObject.CompareTag("Fractal") && other.gameObject.CompareTag("Fractal"))
        {
            Ship otherShip = Ship.GetShipReference(other);

            if (otherShip != null)
            {
                otherShip.Hinder(tower);
            }
        }
    }
}
