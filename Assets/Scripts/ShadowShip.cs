using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class ShadowShip : Ship
{
    public Transform target = null;
    private HashSet<Transform> targets = new HashSet<Transform>();

    private static Vector3 defaultTargetPosition = new Vector3(0, altitude, 0);
    private static float maxDistanceToTarget = 7.5f;
    
    protected override void Awake()
    {
        base.Awake();

        target = GridManager.fractalBaseTransform;
    }

    protected override void InitializeShipType()
    {
        if (!isShipTypeSetByAuthority && Object.HasStateAuthority)
        {
            // TODO This should be conditional
            shipType = ShipType.Tri;

            isShipTypeSetByAuthority = true;
        }
    }


    public override void FixedUpdateNetwork()
    {
        InitializeShipType();

        if (!isReady) return;

        ConstrainY();

        Vector3 move = defaultTargetPosition - transform.position;

        if (target != null)
        {
            move = target.position - transform.position;
        }

        float distanceToTarget = move.magnitude;

        move.Normalize();

        if (distanceToTarget >= maxDistanceToTarget)
        {
            _controller.Move(move);
        }

        Rotate(move);

        if (target != null && !isFiring)
        {
            Fire(true);
        }
        else if (target == null && isFiring)
        {
            Fire(false);
        }
    }

    public void AddTarget(Transform t)
    {
        targets.Add(t);

        target = t;
    }

    public void RemoveTarget(Transform t)
    {
        targets.Remove(t);

        if (targets.Count == 0)
        {
            target = null;
        }
        else if (target == t)
        {
            foreach (Transform tt in targets) {
                target = tt;
                break;
            }
        }
    }
}
