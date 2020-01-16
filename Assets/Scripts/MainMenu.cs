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

    [SerializeField]private NetworkManager _manager;
    // Start is called before the first frame update
    void Awake()
    {
        _manager = _manager.GetComponent<NetworkManager>();
        _HostButton.GetComponent<Button>();
        _ConnectButton.GetComponent<Button>();
        _ServerOnlyButton.GetComponent<Button>();
    }

    // Update is called once per frame
    void Update()
    {
               
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
        if (!NetworkServer.active)
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

    //void OnHostClicked()
    //{
    //    HostSession();
   //* }
    
}
