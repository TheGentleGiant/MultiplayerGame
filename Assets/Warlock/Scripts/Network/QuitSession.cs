using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using UnityEngine.SceneManagement;

public class QuitSession : MonoBehaviour
{
    [SerializeField] private Button _QuitButton = null;
    private NetworkManager _manager = null;
    void Start()
    {
        _QuitButton.GetComponent<Button>();
        _manager = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
    }

    public void OnQuitConnection()
    {
        if (NetworkServer.active || NetworkClient.isConnected)
        {
            _manager.StopHost();
            SceneManager.LoadScene("S_MainMenu", LoadSceneMode.Single);
        }
        
       // _manager = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
    }
}
