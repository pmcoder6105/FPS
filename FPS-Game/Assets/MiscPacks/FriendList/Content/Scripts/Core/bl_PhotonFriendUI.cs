using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class bl_PhotonFriendUI : MonoBehaviour
{
    public Text NameText;
    public Text StatusText;
    public Image StatusImage;
    public GameObject JoinButton;
    private bl_PhotonFriendList Manager;
    private bl_PhotonFriendList.FriendData cacheData;

    int lastStatus = -1;

    public bool Set(bl_PhotonFriendList.FriendData data, bl_PhotonFriendList from)
    {
        cacheData = data;
        gameObject.name = data.Name;
        Manager = from;
        NameText.text = data.Name;
        string st = Manager.StatusData[0].Status;
        Color sc = Manager.StatusData[0].Color;
        int status = 0;
        JoinButton.SetActive(false);
        if (data.Info != null)
        {
            if (data.Info.IsInRoom)
            {
                status = 1;
                st = Manager.StatusData[status].Status;
                sc = Manager.StatusData[status].Color;
                JoinButton.SetActive(true);
            }
            else if (data.Info.IsOnline)
            {
                status = 2;
                st = Manager.StatusData[status].Status;
                sc = Manager.StatusData[status].Color;
            }
        }
        StatusText.text = st;
        StatusImage.color = sc;

        if(lastStatus == -1) { lastStatus = status; }
        else { if(status != lastStatus) { lastStatus = status; return true; } }

        return false;
    }

    /// <summary>
    /// 
    /// </summary>
    public void Remove()
    {
        Manager.RemoveFriend(cacheData.Name);
    }

    /// <summary>
    /// 
    /// </summary>
    public void Join()
    {
        if (cacheData.Info == null) return;
        PhotonNetwork.JoinRoom(cacheData.Info.Room);
    }
}