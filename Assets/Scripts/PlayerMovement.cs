using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
//TODO: sync animations as well 
public class PlayerMovement : NetworkBehaviour
{
    public CharacterController controller;
    public float speed = 25f;
    public float gravity = -9.81f;

    public NetworkTransform netTransform = null;
    public Transform groundCheck;
     public float groundDistance = 0.4f;
     public LayerMask groundMask;
 
     private Animator anim;
     
     private Vector3 velocity;
    private bool isGrounded;
    public bool isLocalPlayer;
    
    void Start()
    {
        anim = GetComponent <Animator>();
        groundCheck = GameObject.Find("Ground").GetComponent<Transform>();
        netTransform = GetComponent<NetworkTransform>();
        isLocalPlayer = netTransform.isLocalPlayer;
    }

    void Update()
    {
        if (isLocalPlayer)
        {
            _PlayerMovement(); 
            Cmd_SendPosition_Rotation(transform.position, transform.rotation, velocity);
        }
       
    }

    private void _PlayerMovement()
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
            
        controller.Move(movement * speed * Time.deltaTime);
            
        velocity.y += gravity * Time.deltaTime;
            
        controller.Move(velocity * Time.deltaTime);
        var magnitude = movement.magnitude;

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
    [Command]
    private void Cmd_SendPosition_Rotation(Vector3 localPosition, Quaternion localRotation, Vector3 velocity)
    {
        Rpc_SendPosition_Rotation(localPosition, localRotation, velocity);
    }

    [ClientRpc]
    private void Rpc_SendPosition_Rotation(Vector3 localPosition, Quaternion localRotation, Vector3 velocity)
    {
        if (!isLocalPlayer)
        {
            transform.localPosition = localPosition;
            transform.rotation = localRotation;
            this.velocity = velocity;
        }
    }
}

