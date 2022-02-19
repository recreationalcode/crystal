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
            // Debug.Log("Target Acquired: " + other.transform.parent.gameObject.name);
            ship.AddTarget(other.transform.parent);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(ship.gameObject.CompareTag("Shadow") && other.gameObject.CompareTag("Fractal") ||
            ship.gameObject.CompareTag("Fractal") && other.gameObject.CompareTag("Shadow"))
        {
            // Debug.Log("Target Lost: " + other.transform.parent.gameObject.name);
            ship.RemoveTarget(other.transform.parent);
        }
    }
}
