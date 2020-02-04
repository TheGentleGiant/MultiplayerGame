using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class Fireball : NetworkBehaviour
{
    public GameObject projectile;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1")&& hasAuthority)
        {
            
            Cmd_SyncFireBall();
        }
    }
    [Command]
    void Cmd_SyncFireBall()
    {
        GameObject fireball = Instantiate(projectile, transform);
        Rigidbody rb = fireball.GetComponent<Rigidbody>();
        fireball.transform.parent = null;
        rb.velocity = transform.forward * 20f;
        NetworkServer.Spawn(fireball);
    }
}
