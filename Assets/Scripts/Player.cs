using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Cinemachine;

public class Player : Ship
{
    private CinemachineVirtualCamera _vcam;

    protected override void Awake()
    {
        base.Awake();

        _vcam = GameObject.Find("VCAM").GetComponent<CinemachineVirtualCamera>();
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

            if (!isFiring && data.isFiring)
            {
                Fire(true);
            }
            
            if (isFiring && !data.isFiring)
            {
                Fire(false);
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
