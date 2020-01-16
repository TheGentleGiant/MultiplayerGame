using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Unity.Collections;

[RequireComponent(typeof(NetworkManager))]
public class ConnectionHUD : MonoBehaviour
{
    private NetworkManager _manager;
    public bool showMenu = true;
    [SerializeField] private Texture2D _texture;
    void Awake()
    {
        _manager = GetComponent<NetworkManager>();
        
    }

    private void OnGUI()
    {
        if (!showMenu)
        {
            return;
        }
        
        //GUILayout.BeginArea(new Rect(new Vector2(10 + Screen.width/2, 10 + Screen.height/2), new Vector2(215, Screen.height/2)));
        
        if (!NetworkClient.isConnected && !NetworkServer.active)
        {
            GUILayout.BeginArea(new Rect(Screen.width/2f- 125f , Screen.height/2f -125f, 250f, 500f), _texture);
            //LAN Host Server    
            if (!NetworkServer.active)
            {
                if (GUILayout.Button("Host LAN Server"))
                {
                    _manager.StartHost();
                }
                //Client
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Connect LAN Server"))
                {
                    _manager.StartClient();
                }
                _manager.networkAddress = GUILayout.TextField(_manager.networkAddress);
                GUILayout.EndHorizontal();
                if (GUILayout.Button("Server Only"))
                {
                    _manager.StartServer();
                } 
            }
            else
            {
                GUILayout.Label("Connecting to " + _manager.networkAddress + "..");
                if (GUILayout.Button("Cancel Connection Attempt"))
                {
                    _manager.StopClient();
                }
            }
            GUILayout.EndArea();
        }
        else
        {
            GUILayout.BeginArea(new Rect(Screen.width-300, Screen.height - 100, 400, 200));
            if (NetworkServer.active)
            {
                GUILayout.Label("Server: Active. Transport: " + Transport.activeTransport);
            }

            if (NetworkClient.isConnected)
            {
                GUILayout.Label("Client: Connected. Address: " + _manager.networkAddress);
            }
            GUILayout.EndArea();
        }
        
        //Create Client/ Add Player
        GUILayout.BeginArea(new Rect(Screen.width-300, Screen.height-40, 300, 200));

        if (NetworkClient.isConnected && !ClientScene.ready)
        {
            if (GUILayout.Button("Client Ready"))
            {
                ClientScene.Ready(NetworkClient.connection);
                if (ClientScene.localPlayer == null)
                {
                    ClientScene.AddPlayer();
                }
            }
        }

        if (NetworkServer.active || NetworkClient.isConnected)
        {
            if (GUILayout.Button("Stop Connection"))
            {
                _manager.StopHost();
            }
        }
        GUILayout.EndArea();
    }
}
