using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class Ship : NetworkBehaviour
{
    public enum Faction
    {
        None,
        Tri, 
        Quad, 
        Penta,
        Hexa
    };
    
    [Networked(OnChanged = nameof(ConstructShipBody), OnChangedTargets = OnChangedTargets.All)]
    public Faction faction { get; set; }
    protected bool isShipTypeSetByAuthority = false;

    public Vector2 axialCoordinates;

    public int rotationSpeed;
    public int health;
    public HealthBar healthBar;
    public int baseDamage = 1;
    private int damageBoost = 0;
    private const int FractalBoost = 1;
    private const int FactionBoost = 2;

    [SerializeField] private GameObject _triPrefab;
    [SerializeField] private GameObject _quadPrefab;
    [SerializeField] private GameObject _pentaPrefab;
    [SerializeField] private GameObject _hexaPrefab;
    private Dictionary<Faction, GameObject> _shipPrefabs;

    [SerializeField] private NetworkPrefabRef _triTowerPrefab;
    [SerializeField] private NetworkPrefabRef _quadTowerPrefab;
    [SerializeField] private NetworkPrefabRef _pentaTowerPrefab;
    [SerializeField] private NetworkPrefabRef _hexaTowerPrefab;
    private Dictionary<Faction, NetworkPrefabRef> _towerPrefabs;

    protected NetworkObject _networkObject;
    protected NetworkCharacterController _controller;
    protected ParticleSystem _particles;
    protected GameObject _shipBody;
    protected bool isFiring = false;
    protected bool isReady = false;
    private GridManager _gridManager;

    public static float altitude = 2f;

    public GridManager GetGridManager()
    {
        if (_gridManager == null)
        {
            _gridManager = GameObject.Find("GridManager(Clone)").GetComponent<GridManager>();
        }

        return _gridManager;
    }

    public static Ship GetShipReference(Object o) {
        if (o is Component)
        {
            return (o as Component).transform.parent.gameObject.GetComponent<Ship>();
        }
        else if (o is GameObject)
        {
            return (o as GameObject).transform.parent.gameObject.GetComponent<Ship>();
        }

        return null;
    }

    protected virtual void Awake()
    {
        _networkObject = GetComponent<NetworkObject>();
        _controller = GetComponent<NetworkCharacterController>();

        _shipPrefabs = new Dictionary<Faction, GameObject>
        {
            {Faction.Tri, _triPrefab},
            {Faction.Quad, _quadPrefab},
            {Faction.Penta, _pentaPrefab},
            {Faction.Hexa, _hexaPrefab}
        };

        _towerPrefabs = new Dictionary<Faction, NetworkPrefabRef>
        {
            {Faction.Tri, _triTowerPrefab},
            {Faction.Quad, _quadTowerPrefab},
            {Faction.Penta, _pentaTowerPrefab},
            {Faction.Hexa, _hexaTowerPrefab}
        };
    }

    public NetworkPrefabRef GetTowerPrefab()
    {
        return _towerPrefabs[faction];
    }

    public override void Spawned()
    {
        if(faction != Faction.None)
        {
            _ConstructShipBody();
        }

        healthBar.InitiateHealth(health);
    }

    public override void Render()
    {
        healthBar.SetHealth(health);
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority, InvokeResim = true)]
    public void RPC_SetShipType(Faction faction)
    {
        this.faction = faction;
    }

    protected virtual void InitializeShipType()
    {
        faction = Faction.Tri;
        isShipTypeSetByAuthority = true;
    }

    public int GetDamage()
    {
        return baseDamage + damageBoost;
    }

    public static void ConstructShipBody(Changed<Ship> changed)
    {
        changed.Behaviour._ConstructShipBody();
    }

    private void _ConstructShipBody()
    {
        isReady = false;

        if (_shipBody != null)
        {
            Destroy(_shipBody);
        }

        _shipBody = Instantiate(_shipPrefabs[faction], transform.position, transform.rotation, transform);

        _particles = _shipBody.gameObject.GetComponent<ParticleSystem>();

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

    public void Boost(Ship ship)
    {
        if (ship == this) return;

        if (faction == ship.faction)
        {
            damageBoost += FactionBoost;
        } else {
            damageBoost += FractalBoost;
        }
    }

    public void Boost(FractalBase fractalBase)
    {
        if (faction == fractalBase.faction || fractalBase.faction == Ship.Faction.None)
        {
            damageBoost += FactionBoost;
        } else {
            damageBoost += FractalBoost;
        }
    }

    public void Hinder(Ship ship)
    {
        if (ship == this) return;

        if (faction == ship.faction)
        {
            damageBoost -= FactionBoost;
        } else {
            damageBoost -= FractalBoost;
        }
    }

    public void Hinder(FractalBase fractalBase)
    {
        if (faction == fractalBase.faction || fractalBase.faction == Ship.Faction.None)
        {
            damageBoost -= FactionBoost;
        } else {
            damageBoost -= FractalBoost;
        }
    }
}
