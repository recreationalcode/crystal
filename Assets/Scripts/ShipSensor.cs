using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipSensor : MonoBehaviour
{
    public ShadowShip ship;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Fractal"))
        {    
            // Debug.Log("Target Acquired: " + other.transform.parent.gameObject.name);
            ship.AddTarget(other.transform.parent);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.CompareTag("Fractal"))
        {
            // Debug.Log("Target Lost: " + other.transform.parent.gameObject.name);
            ship.RemoveTarget(other.transform.parent);
        }
    }
}
