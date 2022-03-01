using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerSensor : MonoBehaviour
{
    public FractalBase fractalBase;

    private void OnTriggerEnter(Collider other)
    {
        if (fractalBase.gameObject.CompareTag("Shadow") && other.gameObject.CompareTag("Fractal") ||
            fractalBase.gameObject.CompareTag("Fractal") && other.gameObject.CompareTag("Shadow"))
        {
            fractalBase.AddTarget(other.transform.parent);
        }

        if (fractalBase.gameObject.CompareTag("Fractal") && other.gameObject.CompareTag("Fractal"))
        {
            Ship otherShip = Ship.GetShipReference(other);

            if (otherShip != null)
            {
                otherShip.Boost(fractalBase);
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (fractalBase.gameObject.CompareTag("Shadow") && other.gameObject.CompareTag("Fractal") ||
            fractalBase.gameObject.CompareTag("Fractal") && other.gameObject.CompareTag("Shadow"))
        {
            fractalBase.AddTarget(other.transform.parent);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (fractalBase.gameObject.CompareTag("Shadow") && other.gameObject.CompareTag("Fractal") ||
            fractalBase.gameObject.CompareTag("Fractal") && other.gameObject.CompareTag("Shadow"))
        {
            fractalBase.RemoveTarget(other.transform.parent);
        }

        if (fractalBase.gameObject.CompareTag("Fractal") && other.gameObject.CompareTag("Fractal"))
        {
            Ship otherShip = Ship.GetShipReference(other);

            if (otherShip != null)
            {
                otherShip.Hinder(fractalBase);
            }
        }
    }
}
