using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class FractalBase : NetworkBehaviour
{
    public int health;
    public HealthBar healthBar;
    public bool isMain = false;
    public Vector2 axialCoordinates;

    protected virtual void Awake()
    {
        healthBar.InitiateHealth(health);
    }

    public override void Render()
    {
        healthBar.SetHealth(health);
    }

    public void Hit(int numCollisionEvents, Vector3 direction)
    {
        health -= numCollisionEvents;

        if (health <= 0)
        {
            if (isMain)
            {
                Runner.Shutdown();
            }

            GridManager.OnTowerDestroyed(axialCoordinates);
            Destroy(gameObject);
        }
    }

    public Transform target = null;
    public Transform defaultTarget = null;
    public int turretRotationSpeed;
    private bool isFiring = false;
    private HashSet<Transform> targets = new HashSet<Transform>();
    [SerializeField] private Transform _turret;
    [SerializeField] private ParticleSystem _particles;

    public void AddTarget(Transform t)
    {
        targets.Add(t);

        if (target == null || target == defaultTarget)
        {
            target = t;
        }
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

    public override void FixedUpdateNetwork()
    {
        // FIXME Refactor to make more sense
        if (_turret == null) return;
        if (target == null)
        {
            target = defaultTarget;

            if (target == null)
            {
                Fire(false);
                return;
            }
        }

        Vector3 targetDirection = target.position - _turret.position;
        targetDirection.Normalize();

        Rotate(_turret, targetDirection, turretRotationSpeed);

        Fire(true);
    }

    protected void Rotate(Transform t, Vector3 direction, int rotationSpeed)
    {
        if (direction != Vector3.zero)
        {
            t.forward = Vector3.Lerp(t.forward, direction, rotationSpeed * Runner.DeltaTime);
        }
    }

    protected void Fire(bool shouldFire)
    {
        if (shouldFire && !isFiring)
        {
            _particles?.Play();

            isFiring = true;
        }
        else if (!shouldFire && isFiring)
        {
            _particles?.Stop();

            isFiring = false;
        }
    }

    private void OnGUI ()
    {
        GUI.Label(new Rect (0,Screen.height - 50,100,50), "Base Health: " + health);
    }
}
