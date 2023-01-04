using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using agora_gaming_rtc;
using System;
using Photon.Pun;

public class VoiceChatManager : MonoBehaviourPunCallbacks
{
    readonly string appID = "d7100fadc1fa4360a3cbd7e22bcd8c6b";

    public static VoiceChatManager Instance;

    IRtcEngine rtcEngine;

    private void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Start()
    {
        rtcEngine = IRtcEngine.GetEngine(appID);

        rtcEngine.OnJoinChannelSuccess += OnJoinChannelSuccess;
        rtcEngine.OnLeaveChannel += OnLeaveChannel;
        rtcEngine.OnError += OnError;
    }

    private void OnError(int error, string msg)
    {
        Debug.LogError("Error with Agora: " + msg);
    }

    private void OnLeaveChannel(RtcStats stats)
    {
        Debug.Log("Left Channel with duration " + stats.duration);
    }

    private void OnJoinChannelSuccess(string channelName, uint uid, int elapsed)
    {
        Debug.Log("Joined channel " + channelName);
    }

    public override void OnJoinedRoom()
    {
        rtcEngine.JoinChannel(PhotonNetwork.CurrentRoom.Name);
    }

    public override void OnLeftRoom()
    {
        rtcEngine.LeaveChannel();
    }

    private void OnDestroy()
    {
        IRtcEngine.Destroy();
    }
}
