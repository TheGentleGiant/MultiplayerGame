using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class FireBallMovement : NetworkBehaviour
{
    public float Speed = 5f;
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.Translate(Vector3.forward *Speed);
    }
}
