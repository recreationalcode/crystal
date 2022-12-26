using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Cinemachine;

public class Player : Ship
{
    private CinemachineVirtualCamera _vcam;
    
    private HashSet<NetworkObject> towers = new HashSet<NetworkObject>();
    private Airspace targetAirspace = null;
    private Cell targetCell = null;
    private int availableTowerCount = 5;

    public bool HasAvailableTowers()
    {
        return (availableTowerCount - towers.Count) > 0;
    }

    private void PlaceTower()
    {
        towers.Add(GetGridManager().PlaceTower(axialCoordinates, GetTowerPrefab()));
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

        // _vcam = GameObject.Find("VCAM").GetComponent<CinemachineVirtualCamera>();
    }

    private void Start()
    {
        if(Object.HasInputAuthority)
        {
            // _vcam.Follow = transform;
        }
    }

    protected override void InitializeShipType()
    {
        if (!isShipTypeSetByAuthority && Object.HasInputAuthority)
        {
            // TODO Change this to some kind of player preference or ideally from the player NFT!
            if (Runner.IsServer)
            {
                faction = Faction.Hexa;
            }
            else
            {
                RPC_SetShipType(Faction.Quad);
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
            GUI.Label(new Rect (0,50,100,50), "Damage: " + GetDamage());
        }
    }
}
