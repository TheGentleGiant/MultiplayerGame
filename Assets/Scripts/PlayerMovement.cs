using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    public CharacterController controller;

    [SyncVar]public float speed = 12f;
    [SyncVar]public float gravity = -9.81f;


    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    private Animator anim;
    
    [SyncVar]Vector3 velocity;
    bool isGrounded;

    void Start()
    {
        anim = GetComponent <Animator>();
        groundCheck = GameObject.Find("Ground").GetComponent<Transform>();
    }

    void Update()
    {
        if (hasAuthority)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                anim.SetTrigger("Cast");
            }
            if (Input.GetButtonDown("Fire2"))
            {
                anim.SetTrigger("Push");
            }
            
            isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

            var input = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));

            if (input.magnitude > 1f)
                input.Normalize();

            var movement = input * speed * Time.deltaTime;
            var magnitude = movement.magnitude;

            controller.Move(movement * speed * Time.deltaTime);

            velocity.y += gravity * Time.deltaTime;

            controller.Move(velocity * Time.deltaTime);

            if (magnitude > 0.1)
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(movement.normalized, Vector3.up), 0.25f);
                anim.SetBool("isRunning", true);
            }
            else if (magnitude == 0)
            {
                anim.SetBool("isRunning", false);
            }
        }
        
    }
}
