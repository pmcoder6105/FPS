using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;
using System.Linq;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
    [SerializeField] GameObject playerModelSkin, lilbean;
    public Material colorChosen;
    [SerializeField] GameObject titleMenu, loadingMenu, roomMenu;


    List<RoomInfo> fullRoomList = new();
    List<RoomListItem> roomListItems = new();

    public FlexibleColorPicker fcp;

    public GameObject customizeBeanModel;

    public Button gameMode;


    private void Awake()
    {
        Instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Connecting to Master");
        if (!PhotonNetwork.IsConnectedAndReady)
        {
            PhotonNetwork.ConnectUsingSettings();
        }
        gameMode.onClick.AddListener(ReturnToGameMode);
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
        DisplayPlayerInMainMenu();

    }

    private void DisplayPlayerInMainMenu()
    {
        if (titleMenu.activeInHierarchy == true)
        {
            playerModelSkin.SetActive(true);
            playerModelSkin.transform.GetChild(0).transform.gameObject.GetComponent<MeshRenderer>().material = colorChosen;
            lilbean.transform.gameObject.GetComponent<MeshRenderer>().material = colorChosen;
            //playerModelSkin.transform.GetChild(0).GetChild(1).transform.gameObject.GetComponent<MeshRenderer>().material = colorChosen
        }
        else
        {
            playerModelSkin.SetActive(false);
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
        loadingMenu.SetActive(true);
        roomMenu.SetActive(false);
        
        Invoke("LoadGame", 3f);
        //PhotonNetwork.LoadLevel(1);
    }

    private void LoadGame()
    {
        PhotonNetwork.LoadLevel(mapChooseMenu.GetComponent<MapChooser>().mapName);
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
    
    public void ReturnToGameMode()
    {
        Destroy(GameObject.Find("VoiceChatManager"));
        Destroy(GameObject.Find("FirebaseManager"));
        Destroy(GameObject.Find("AccessoriesManager"));
        Destroy(GameObject.Find("LevelUpManager"));
        Destroy(GameObject.Find("RoomManager"));
        SceneManager.LoadScene("LoadingScene");
        
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        RoomInfo newRoom = null;
        foreach (RoomInfo updatedRoom in roomList)
        {
            RoomInfo existingRoom = fullRoomList.Find(x => x.Name.Equals(updatedRoom.Name)); // Check to see if we have that room already
            if (existingRoom == null) // WE DO NOT HAVE IT
            {
                fullRoomList.Add(updatedRoom); // Add the room to the full room list
                if (newRoom == null)
                {
                    newRoom = updatedRoom;
                }
            }
            else if (updatedRoom.RemovedFromList || updatedRoom.PlayerCount == 0) // WE DO HAVE IT, so check if it has been removed
            {
                fullRoomList.Remove(existingRoom); // Remove it from our full room list
            }
        }
        RenderRoomList();
    }

    void RenderRoomList()
    {
        RemoveRoomList();
        foreach (RoomInfo roomInfo in fullRoomList)
        {
            if (roomInfo.PlayerCount == 0 || roomInfo.RemovedFromList)
                continue;
            RoomListItem roomListItem = Instantiate(roomListItemPrefab, roomListContent).GetComponent<RoomListItem>();
            roomListItem.SetUp(roomInfo);
            roomListItems.Add(roomListItem);
        }
    }

    void RemoveRoomList()
    {
        foreach (RoomListItem roomListItem in roomListItems)
        {
            Destroy(roomListItem.gameObject);
        }
        roomListItems.Clear();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Instantiate(playerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(newPlayer);
    }
}
