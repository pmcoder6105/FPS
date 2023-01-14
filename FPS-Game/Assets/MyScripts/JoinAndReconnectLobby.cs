using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class JoinAndReconnectLobby : MonoBehaviourPunCallbacks
{    
    [SerializeField] GameObject reconnectBtn;
    [SerializeField] GameObject playerViewerParent;

    private static JoinAndReconnectLobby _singleton;

    private void Awake()
    {
        Singleton = this;
        DontDestroyOnLoad(this.gameObject);
    }

    public static JoinAndReconnectLobby Singleton
    {
        get => _singleton;
        private set
        {
            if (_singleton == null)
            {
                _singleton = value;
            }
            else if (_singleton != value)
            {
                Debug.Log($"{nameof(AccessoriesManager)} instance already exists, destroying object!");
                Destroy(value.gameObject);
            }
            Debug.Log("Singleton called");
        }
    }

    public void OpenTitle()
    {
        StartCoroutine(nameof(Reconnect));
    }

    private IEnumerator Reconnect()
    {
        //while (this.gameObject.activeSelf)
        //{
        //    yield return new WaitForSeconds(3f);
        //    reconnectBtn.SetActive(true);
        //}

        yield return new WaitForSeconds(3f);

        MenuManager.Instance.OpenMenu("title");
        playerViewerParent.SetActive(true);
    }

    public void Btn_Reconnect()
    {
        PhotonNetwork.LoadLevel("Menu");
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master");
        PhotonNetwork.JoinLobby();
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public override void OnJoinedLobby()
    {
        MenuManager.Instance.OpenMenu("title");
        Debug.Log("Joined Lobby");
        playerViewerParent.SetActive(true);
    }
}
