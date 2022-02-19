using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class Ship : NetworkBehaviour
{
    public enum ShipType
    {
        None,
        Tri, 
        Quad, 
        Penta,
        Hexa
    };
    
    [Networked(OnChanged = nameof(ConstructShipBody), OnChangedTargets = OnChangedTargets.All)]
    public ShipType shipType { get; set; }
    protected bool isShipTypeSetByAuthority = false;

    public Vector2 axialCoordinates;

    public int rotationSpeed;
    public int health;
    public HealthBar healthBar;

    [SerializeField] private GameObject _triPrefab;
    [SerializeField] private GameObject _quadPrefab;
    [SerializeField] private GameObject _pentaPrefab;
    [SerializeField] private GameObject _hexaPrefab;
    private Dictionary<Ship.ShipType, GameObject> _shipPrefabs;

    protected NetworkObject _networkObject;
    protected NetworkCharacterController _controller;
    [SerializeField] protected ParticleSystem _particles;
    protected bool isFiring = false;
    protected bool isReady = false;
    private GridManager _gridManager;

    public static float altitude = 2f;

    protected GridManager GetGridManager()
    {
        if (_gridManager == null)
        {
            _gridManager = GameObject.Find("GridManager(Clone)").GetComponent<GridManager>();
        }

        return _gridManager;
    }

    protected virtual void Awake()
    {
        _networkObject = GetComponent<NetworkObject>();
        _controller = GetComponent<NetworkCharacterController>();

        _shipPrefabs = new Dictionary<Ship.ShipType, GameObject>
        {
            {ShipType.Tri, _triPrefab},
            {ShipType.Quad, _quadPrefab},
            {ShipType.Penta, _pentaPrefab},
            {ShipType.Hexa, _hexaPrefab}
        };

        healthBar.InitiateHealth(health);
    }

    public override void Spawned()
    {
        if(shipType != ShipType.None)
        {
            _ConstructShipBody();
        }
    }

    public override void Render()
    {
        healthBar.SetHealth(health);
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority, InvokeResim = true)]
    public void RPC_SetShipType(ShipType st)
    {
        shipType = st;
    }

    protected virtual void InitializeShipType()
    {
        shipType = ShipType.Tri;
        isShipTypeSetByAuthority = true;
    }

    public static void ConstructShipBody(Changed<Ship> changed)
    {
        changed.Behaviour._ConstructShipBody();
    }

    private void _ConstructShipBody()
    {
        GameObject shipBody = Instantiate(_shipPrefabs[shipType], transform.position, transform.rotation, transform);

        _particles = shipBody.gameObject.GetComponent<ParticleSystem>();

        isReady = true;
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

    public void Hit(int numCollisionEvents, Vector3 direction)
    {
        health -= numCollisionEvents;

        if (health <= 0)
        {
            Runner.Despawn(_networkObject);
        }
    }
}
