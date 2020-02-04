using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class MainMenu : MonoBehaviour
{
    [SerializeField] private Button _HostButton = null;
    [SerializeField] private Button _ConnectButton = null;
    [SerializeField] private Button _ServerOnlyButton = null;
    private NetworkManager _manager;
    [SerializeField]private GameObject _managerPrefab;

    void Awake()
    {
        _HostButton.GetComponent<Button>();
        _ConnectButton.GetComponent<Button>();
        _ServerOnlyButton.GetComponent<Button>();
    }

    private void Start()
    {
        if (NetworkManager.singleton == null)
        {
            GameObject _tempManager = GameObject.Instantiate(_managerPrefab);
            _tempManager.name = "NetworkManager";
        }
        _manager = _managerPrefab.GetComponent<NetworkManager>();
    }

    public void HostLANServer()
    {
        if (!NetworkClient.isConnected && !NetworkServer.active)
        {
            _manager.StartHost();
            Debug.Log("Started Server!");
        }
    }


    public void ConnectToLANServer()
    {
        if (!NetworkClient.isConnected && !NetworkServer.active)
        {
            _manager.StartClient();
        }
    }

    public void HostServerOnly()
    {
        if (!NetworkServer.active)
        {
            _manager.StartServer();
        }
    }
}
