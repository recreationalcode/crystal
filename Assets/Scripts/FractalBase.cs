using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class FractalBase : NetworkBehaviour
{
    public int health;
    public HealthBar healthBar;

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
            Runner.Shutdown();
        }
    }

    private void OnGUI ()
    {
        GUI.Label(new Rect (0,Screen.height - 50,100,50), "Base Health: " + health);
    }
}
