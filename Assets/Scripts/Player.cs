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
        if(_networkObject.HasInputAuthority)
        {
            _vcam.Follow = transform;
        }
    }
        
    public override void FixedUpdateNetwork()
    {
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
