﻿using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
public class PlayerMovement : NetworkBehaviour
{
    public CharacterController controller;
    public float speed = 25f;
    public float gravity = -9.81f;
    [SerializeField] private float lerpRate = 1f;
    public NetworkTransform netTransform = null;
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    private Animator anim;
    private Vector3 velocity;
    private bool isGrounded;
    public bool isLocalPlayer;
    private Vector3 positionToSync;
    private Vector3 movement;
    private float magnitude = 0.0f;
    private bool isMoving = false;
    private PlayerCast cast;
    private LifeCycle life;

    void Start()
    {
        anim = GetComponent <Animator>();
        cast = GetComponent<PlayerCast>();
        life = GetComponent<LifeCycle>();
        groundCheck = GameObject.Find("Ground").GetComponent<Transform>();
        netTransform = GetComponent<NetworkTransform>();
        isLocalPlayer = netTransform.isLocalPlayer;
        positionToSync = transform.position;
    }

    void FixedUpdate()
    {
        if (isLocalPlayer)
        {
            _PlayerMovement(); 
            Cmd_SendPosition_Rotation(transform.position, transform.rotation, velocity, isMoving);
            LerpPosition();
        }
    }

    private void LerpPosition()
    {
        if (!isLocalPlayer)
        {
//            transform.position = Vector3.Lerp(transform.position, positionToSync, lerpRate);
            Vector3 deltaPos = positionToSync - transform.position;
            transform.position = deltaPos * 0.3f;
        }
    }

    private void _PlayerMovement()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        var input = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));

        // Block movement input when casting or when dead
        if ((cast != null && cast.IsCasting) || (life != null && life.IsDead))
            input = Vector3.zero;

        if (input.magnitude > 1f)
            input.Normalize();
            
        movement = input * speed * Time.deltaTime;
            
        controller.Move(movement * speed * Time.deltaTime);
        velocity.y += gravity * Time.deltaTime;
            
        controller.Move(velocity * Time.deltaTime);
        magnitude = movement.magnitude;

        velocity *= 0.96f;

        if (magnitude > 0.1)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(movement.normalized, Vector3.up), 0.25f);
            anim.SetBool("isRunning", true);
            isMoving = true;
        }
        else if (magnitude == 0)
        {
            anim.SetBool("isRunning", false);
            isMoving = false;
        }
    }

    public void Knockback(Vector3 force)
    {
        var connection = connectionToClient;

        // Server has to tell the client to add knockback if it doesn't have authority
        if (isServer && !hasAuthority && connection != null)
        {
            TargetRpc_Knockback(connection, force);
            return;
        }

        velocity += force;
    }

    [TargetRpc]
    private void TargetRpc_Knockback(NetworkConnection connection, Vector3 force) => Knockback(force);

    [Command]
    private void Cmd_SendPosition_Rotation(Vector3 localPosition, Quaternion localRotation, Vector3 velocity, bool _isMoving)
    {
        Rpc_SendPosition_Rotation(localPosition, localRotation, velocity, _isMoving);
    }
    
    [ClientRpc]
    private void Rpc_SendPosition_Rotation(Vector3 localPosition, Quaternion localRotation, Vector3 velocity, bool _isMoving)
    {
        if (!isLocalPlayer)
        {
            transform.localPosition = localPosition;
            transform.rotation = localRotation;
            this.velocity = velocity;
            positionToSync = localPosition;
            anim.SetBool("isRunning", _isMoving);
        }
    }
}