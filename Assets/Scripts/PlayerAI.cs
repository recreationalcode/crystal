using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Cinemachine;

public class PlayerAI : ShadowShip
{
    protected override void InitializeShipType()
    {
        if (!isShipTypeSetByAuthority && Object.HasStateAuthority)
        {
            if (Random.value > 0.5f)
            {
                shipType = ShipType.Tri;
            }
            else
            {
                shipType = ShipType.Quad;
            }

            isShipTypeSetByAuthority = true;
        }
    }
}
