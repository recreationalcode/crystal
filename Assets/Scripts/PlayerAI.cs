using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Cinemachine;

public class PlayerAI : ShadowShip
{
    protected new static float fireDistanceToTarget = 15f;
    protected new static float maxDistanceToTarget = 10f;

    private static float timeBetweenDefaultTargetChanges = 5f;
    private float lastDefaultTargetChangeTimestamp;

    protected override void InitializeHealth()
    {
        return;
    }

    protected override void InitializeDamage()
    {
        return;
    }

    protected override void InitializeTarget()
    {
        ChangeDefaultTarget();

        target = defaultTarget;
    }

    protected override void InitializeShipType()
    {
        if (!isShipTypeSetByAuthority && Object.HasStateAuthority)
        {
            float r = Random.value;

            if (r <= 0.40f)
            {
                faction = Faction.Tri;
            }
            else if (r <= 0.80f)
            {
                faction = Faction.Quad;
            }
            else
            {
                faction = Faction.Hexa;
            }

            isShipTypeSetByAuthority = true;
        }
    }

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();

        if (Time.realtimeSinceStartup - lastDefaultTargetChangeTimestamp >= timeBetweenDefaultTargetChanges)
        {
            ChangeDefaultTarget();
        }
    }

    private void ChangeDefaultTarget()
    {
        float r = Random.value;

        if (r <= 0.0f)
        {
            defaultTarget = Runner.GetPlayerObject(Runner.LocalPlayer).transform;
        }
        else if (r <= 0.0f)
        {
            defaultTarget = GridManager.fractalBaseTransform;
        }
        else
        {
            Vector2 axialCoordinates = GridManager.shadowOutline[Random.Range(0, GridManager.shadowOutline.Count - 1)];
            defaultTarget = GridManager.gridCells[Vector2Int.RoundToInt(axialCoordinates)].transform;
        }

        lastDefaultTargetChangeTimestamp = Time.realtimeSinceStartup;
    }
}
