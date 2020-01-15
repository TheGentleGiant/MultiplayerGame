using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : MonoBehaviour
{
    public GameObject projectile;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            GameObject fireball = Instantiate(projectile, transform);
            Rigidbody rb = fireball.GetComponent<Rigidbody>();
            fireball.transform.parent = null;
            rb.velocity = transform.forward * 20f;
        }

    }
}
