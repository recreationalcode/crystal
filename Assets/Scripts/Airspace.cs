using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Airspace : MonoBehaviour
{
    public Cell cell;

    private void OnTriggerEnter(Collider other)
    {
        Ship ship = other.transform.parent.gameObject.GetComponent<Ship>();

        if (ship == null) return;

        if(other.gameObject.CompareTag("Fractal"))
        {    
            cell.Crystallize(ship.shipType);   
        } else if(other.gameObject.CompareTag("Shadow"))
        {    
            cell.Fracture();   
        }
    }
}
