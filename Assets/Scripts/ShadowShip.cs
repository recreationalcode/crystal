using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class ShadowShip : Ship
{
    public Transform target = null;
    public Transform defaultTarget = null;
    
    protected static float fireDistanceToTarget = 10f;
    protected static float maxDistanceToTarget = 7.5f;

    private HashSet<Transform> targets = new HashSet<Transform>();

    public override void Spawned()
    {
        base.Spawned();

        InitializeTarget();
    }

    protected virtual void InitializeTarget()
    {
        defaultTarget = GridManager.fractalBaseTransform;
        target = defaultTarget;
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

    private bool ShouldFire(float distance)
    {
        return target != null && distance <= fireDistanceToTarget && (
            gameObject.CompareTag("Shadow") && target.gameObject.CompareTag("Fractal") ||
            gameObject.CompareTag("Fractal") && target.gameObject.CompareTag("Shadow"));
    }


    public override void FixedUpdateNetwork()
    {
        InitializeShipType();

        if (!isReady) return;

        ConstrainY();

        Vector3 move = defaultTarget.position - transform.position;

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

        Fire(ShouldFire(distanceToTarget));
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
            target = defaultTarget;
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
