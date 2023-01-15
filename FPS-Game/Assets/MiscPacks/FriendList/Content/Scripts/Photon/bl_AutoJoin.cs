using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using Photon.Pun;

public class bl_AutoJoin : MonoBehaviourPunCallbacks
{

    public string PlayerName = "Player";
    public byte Version = 1;
    public Text PlayerNameText;
    [Space(5)]
    public GameObject PlayerNameInput;

    private void Awake()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    public void SendPlayerName(InputField field)
    {
        string t = field.text;
        if (string.IsNullOrEmpty(t))
            return;

        //IMPORTANT
        //in PUN 2 you have to set the UserID before connect in order to use Friend list
        PhotonNetwork.NickName = t;
        //so if you have your own lobby connection script, don't forget to set 
        //PhotonNetwork.AuthValues.UserId = PhotonNetwork.NickName;
        PhotonNetwork.AuthValues =  new AuthenticationValues();
        PhotonNetwork.AuthValues.UserId = PhotonNetwork.NickName;
        PhotonNetwork.ConnectUsingSettings();

        PlayerNameInput.SetActive(false);
    }
    // the following methods are implemented to give you some context. re-implement them as needed.

    public virtual void OnFailedToConnectToPhoton(DisconnectCause cause)
    {
        Debug.LogError("Cause: " + cause);
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        PlayerNameText.text = string.Format("YOUR NAME: {0}\nUSER ID: {1}", PhotonNetwork.NickName, PhotonNetwork.LocalPlayer.UserId);
    }
}