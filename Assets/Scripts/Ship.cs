using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class Ship : NetworkBehaviour
{
    public enum ShipType
     {
        Tri, 
        Quad, 
        Penta,
        Hexa
     };
    
    public ShipType shipType;
    public int rotationSpeed;
    public int health;

    protected NetworkObject _networkObject;
    protected NetworkCharacterController _controller;
    protected ParticleSystem _particles;
    protected bool isFiring = false;

    public static float altitude = 2f;


    protected virtual void Awake()
    {
        _networkObject = GetComponent<NetworkObject>();
        _controller = GetComponent<NetworkCharacterController>();
        _particles = GetComponent<ParticleSystem>();
    }

    protected void ConstrainY()
    {
        transform.position = new Vector3(transform.position.x, altitude, transform.position.z);
    }

    protected void Rotate(Vector3 direction)
    {
        if (direction != Vector3.zero)
        {
            transform.forward = Vector3.Lerp(transform.forward, new Vector3(direction.x, 0, direction.z), rotationSpeed * Runner.DeltaTime);
        }
    }

    protected void Fire(bool shouldFire)
    {
        if (shouldFire)
        {
            _particles.Play();

            isFiring = true;
        }
        else
        {
            _particles.Stop();

            isFiring = false;
        }
    }

    public void Hit(int numCollisionEvents, Vector3 direction)
    {
        health -= numCollisionEvents;

        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }
}
