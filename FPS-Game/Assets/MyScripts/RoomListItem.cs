using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using TMPro;

public class RoomListItem : MonoBehaviour
{
    [SerializeField] TMP_Text text, playerListAmount;

    public RoomInfo info;

    public void SetUp(RoomInfo _info)
    {
        info = _info;
        text.text = info.Name;
    }

    private void Update()
    {
        if (PhotonNetwork.PlayerList.Length == 1)
        {
            playerListAmount.text = "1/2";
        }
        if (PhotonNetwork.PlayerList.Length == 2)
        {
            playerListAmount.text = "MAX";
        }
    }

    public void OnClick()
    {
        RoomOptions options = new RoomOptions();
        options.MaxPlayers = 2;
        if (PhotonNetwork.PlayerList.Length > 2)
        {
            Launcher.Instance.JoinRoom(info);
        }
        
    }
}
