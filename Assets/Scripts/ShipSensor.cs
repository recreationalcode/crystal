using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipSensor : MonoBehaviour
{
    public ShadowShip ship;

    private void OnTriggerEnter(Collider other)
    {
        if(ship.gameObject.CompareTag("Shadow") && other.gameObject.CompareTag("Fractal") ||
            ship.gameObject.CompareTag("Fractal") && other.gameObject.CompareTag("Shadow"))
        {    
            ship.AddTarget(other.transform.parent);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(ship.gameObject.CompareTag("Shadow") && other.gameObject.CompareTag("Fractal") ||
            ship.gameObject.CompareTag("Fractal") && other.gameObject.CompareTag("Shadow"))
        {
            ship.RemoveTarget(other.transform.parent);
        }
    }
}
