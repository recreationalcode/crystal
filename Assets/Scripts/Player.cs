using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Cinemachine;

public class Player : Ship
{
    private CinemachineVirtualCamera _vcam;

    [SerializeField] private NetworkPrefabRef _triTowerPrefab;
    [SerializeField] private NetworkPrefabRef _quadTowerPrefab;
    [SerializeField] private NetworkPrefabRef _pentaTowerPrefab;
    [SerializeField] private NetworkPrefabRef _hexaTowerPrefab;
    private Dictionary<Ship.ShipType, NetworkPrefabRef> _towerPrefabs;
    private HashSet<NetworkObject> _towers = new HashSet<NetworkObject>();
    private Airspace targetAirspace = null;
    private Cell targetCell = null;
    private int availableTowerCount = 5;

    public bool HasAvailableTowers()
    {
        return (availableTowerCount - _towers.Count) > 0;
    }

    private void PlaceTower()
    {
        _towers.Add(GetGridManager().PlaceTower(axialCoordinates, _towerPrefabs[shipType]));
        SetTargetAirspace(null);
    }

    private void SetTargetAirspace(Airspace airspace)
    {
        targetAirspace = airspace;
        targetCell = airspace != null ? airspace.cell : null;
        axialCoordinates = targetCell != null ? targetCell.GetAxialCoordinates() : Vector2.zero;
    }

    public void EnableTowerPlacement(Airspace airspace)
    {
        // TODO Address situation where player never fully left airspace but entered another one (airspace stack?) 

        if (targetAirspace != null)
        {
            targetAirspace.SetActive(false);
        }

        SetTargetAirspace(airspace);

        airspace.SetActive(true);
    }

    public void DisableTowerPlacement(Airspace airspace)
    {
        airspace.SetActive(false);

        if (targetAirspace == null) return;

        if(GameObject.ReferenceEquals(airspace.gameObject, targetAirspace.gameObject))
        {
            SetTargetAirspace(null);
        }
    }

    private bool CanPlaceTower()
    {
        return targetAirspace != null && HasAvailableTowers();
    }

    protected override void Awake()
    {
        base.Awake();

        _vcam = GameObject.Find("VCAM").GetComponent<CinemachineVirtualCamera>();

        _towerPrefabs = new Dictionary<Ship.ShipType, NetworkPrefabRef>
        {
            {ShipType.Tri, _triTowerPrefab},
            {ShipType.Quad, _quadTowerPrefab},
            {ShipType.Penta, _pentaTowerPrefab},
            {ShipType.Hexa, _hexaTowerPrefab}
        };
    }

    private void Start()
    {
        if(Object.HasInputAuthority)
        {
            _vcam.Follow = transform;
        }
    }

    protected override void InitializeShipType()
    {
        if (!isShipTypeSetByAuthority && Object.HasInputAuthority)
        {
            // TODO Change this to some kind of player preference or ideally from the player NFT!
            if (Runner.IsServer)
            {
                shipType = ShipType.Tri;
            }
            else
            {
                RPC_SetShipType(ShipType.Quad);
            }

            isShipTypeSetByAuthority = true;
        }
    }

        
    public override void FixedUpdateNetwork()
    {
        InitializeShipType();

        if (!isReady) return;

        ConstrainY();

        if (GetInput(out NetworkInputData data))
        {
            Vector3 move = data.direction;
            move.Normalize();

            _controller.Move(move);

            Rotate(move);

            Fire(data.isFiring);

            if (CanPlaceTower() && data.hasPlacedTower)
            {
                PlaceTower();
            }

        }
    }

    private void OnGUI ()
    {
        if(_networkObject.HasInputAuthority)
        {
            GUI.Label(new Rect (0,0,100,50), "Health: " + health);
        }
    }
}
