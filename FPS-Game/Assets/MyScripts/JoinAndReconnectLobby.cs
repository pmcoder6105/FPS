using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class JoinAndReconnectLobby : MonoBehaviourPunCallbacks
{    
    [SerializeField] GameObject reconnectBtn;
    [SerializeField] GameObject playerViewerParent;

    private void Start()
    {
        if (Time.realtimeSinceStartup > 5)
        {
            StartCoroutine(nameof(Reconnect));
        }
    }

    private IEnumerator Reconnect()
    {
        while (this.gameObject.activeSelf)
        {
            yield return new WaitForSeconds(3f);
            reconnectBtn.SetActive(true);
        }
    }

    public void Btn_Reconnect()
    {
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
