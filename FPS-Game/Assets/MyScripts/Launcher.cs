using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;
using System.Linq;

public class Launcher : MonoBehaviourPunCallbacks
{
    public static Launcher Instance;

    [SerializeField] TMP_InputField roomNameInputField;
    [SerializeField] TMP_Text errorText;
    [SerializeField] TMP_Text roomNameText;
    [SerializeField] Transform roomListContent;
    [SerializeField] Transform playerListContent;
    [SerializeField] GameObject roomListItemPrefab;
    [SerializeField] GameObject playerListItemPrefab;
    [SerializeField] GameObject startGameButton;
    [SerializeField] GameObject mapChooseMenu;    
    [SerializeField] GameObject nonClientWaitingText;

    [SerializeField] GameObject playerViewerParent;
    [SerializeField] GameObject playerModelSkin;
    //[SerializeField] GameObject applyColorGameObject;
    public Material colorChosen;
    ApplyColor colorSystem;
    [SerializeField] GameObject titleMenu;


    private void Awake()
    {
        Instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Connecting to Master");
        PhotonNetwork.ConnectUsingSettings();
        //colorSystem = applyColorGameObject.GetComponent<ApplyColor>();
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

    // Update is called once per frame
    void Update()
    {
        //if (PlayerPrefs.GetInt("PillColor") == 0)
        //{
        //    PlayerPrefs.SetInt("PillColor", 1);
        //}

        if (titleMenu.activeInHierarchy == true)
        {
            playerModelSkin.SetActive(true);
            playerModelSkin.transform.GetChild(0).GetChild(0).transform.gameObject.GetComponent<MeshRenderer>().material = colorChosen;
            playerModelSkin.transform.GetChild(0).GetChild(1).transform.gameObject.GetComponent<MeshRenderer>().material = colorChosen;
            //if (PlayerPrefs.GetInt("PillColor") == 1)
            //{
            //    Debug.Log("player1");
            //    player1.SetActive(true);
            //    player2.SetActive(false);
            //    player3.SetActive(false);
            //    player4.SetActive(false);
            //    player5.SetActive(false);
            //    player6.SetActive(false);
            //}
            //if (PlayerPrefs.GetInt("PillColor") == 2)
            //{
            //    Debug.Log("player2");
            //    player1.SetActive(false);
            //    player2.SetActive(true);
            //    player3.SetActive(false);
            //    player4.SetActive(false);
            //    player5.SetActive(false);
            //    player6.SetActive(false);
            //}
            //if (PlayerPrefs.GetInt("PillColor") == 3)
            //{
            //    Debug.Log("player3");
            //    player1.SetActive(false);
            //    player2.SetActive(false);
            //    player3.SetActive(true);
            //    player4.SetActive(false);
            //    player5.SetActive(false);
            //    player6.SetActive(false);
            //}
            //if (PlayerPrefs.GetInt("PillColor") == 4)
            //{
            //    Debug.Log("player4");
            //    player1.SetActive(false);
            //    player2.SetActive(false);
            //    player3.SetActive(false);
            //    player4.SetActive(true);
            //    player5.SetActive(false);
            //    player6.SetActive(false);
            //}
            //if (PlayerPrefs.GetInt("PillColor") == 5)
            //{
            //    Debug.Log("player5");
            //    player1.SetActive(false);
            //    player2.SetActive(false);
            //    player3.SetActive(false);
            //    player4.SetActive(false);
            //    player5.SetActive(true);
            //    player6.SetActive(false);
            //}
            //if (PlayerPrefs.GetInt("PillColor") == 6)
            //{
            //    Debug.Log("player6");
            //    player1.SetActive(false);
            //    player2.SetActive(false);
            //    player3.SetActive(false);
            //    player4.SetActive(false);
            //    player5.SetActive(false);
            //    player6.SetActive(true);
            //}

        } else
        {
            playerModelSkin.SetActive(false);
            //player2.SetActive(false);
            //player3.SetActive(false);
            //player4.SetActive(false);
            //player5.SetActive(false);
            //player6.SetActive(false);
        }
    }

    public void CreateRoom()
    {
        if (string.IsNullOrEmpty(roomNameInputField.text))
        {
            return;
        }
        PhotonNetwork.CreateRoom(roomNameInputField.text);
        MenuManager.Instance.OpenMenu("loading");
    }

    public override void OnJoinedRoom()
    {
        MenuManager.Instance.OpenMenu("room");
        roomNameText.text = PhotonNetwork.CurrentRoom.Name;
        Player[] players = PhotonNetwork.PlayerList;
        foreach (Transform child in playerListContent)
        {
            Destroy(child.gameObject);
        }
        for (int i = 0; i < players.Count(); i++)
        {
            Instantiate(playerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(players[i]);
        }

        startGameButton.SetActive(PhotonNetwork.IsMasterClient);
        mapChooseMenu.SetActive(PhotonNetwork.IsMasterClient);

        if (PhotonNetwork.IsMasterClient == false)
        {
            nonClientWaitingText.SetActive(true);
        }
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        startGameButton.SetActive(PhotonNetwork.IsMasterClient);
        mapChooseMenu.SetActive(PhotonNetwork.IsMasterClient);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        errorText.text = "Room Creation Failed: " + message;
        MenuManager.Instance.OpenMenu("error");
    }

    public void StartGame()
    {
        PhotonNetwork.LoadLevel(mapChooseMenu.GetComponent<MapChooser>().mapName);
        //PhotonNetwork.LoadLevel(1);
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        MenuManager.Instance.OpenMenu("loading");
    }

    public void JoinRoom(RoomInfo info)
    {
        PhotonNetwork.JoinRoom(info.Name);
        MenuManager.Instance.OpenMenu("loading");        
    }

    public override void OnLeftRoom()
    {
        MenuManager.Instance.OpenMenu("title");
    }

    //List<RoomInfo> fullRoomList = new List<RoomInfo>();
    //List<RoomListItem> roomListItems = new List<RoomListItem>();

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (Transform trans in roomListContent)
        {
            Destroy(trans.gameObject);
        }
        for (int i = 0; i < roomList.Count; i++)
        {
            if (roomList[i].RemovedFromList)
                continue;
            Instantiate(roomListItemPrefab, roomListContent).GetComponent<RoomListItem>().SetUp(roomList[i]);
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Instantiate(playerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(newPlayer);
    }
}
