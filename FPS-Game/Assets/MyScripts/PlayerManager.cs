using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;
using Photon.Realtime;
using System.Linq;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using TMPro;
using UnityEngine.UI;
using Cinemachine;

public class PlayerManager : MonoBehaviour
{
    PhotonView PV;

    public GameObject controller;

    int kills;
    int deaths;

    [SerializeField] GameObject killTextNotification; // a kill (player killed player) that instantiates whenever a victim dies
    GameObject killTextNotificationHolder; // kill text notification holder empty gameobject
    [SerializeField] GameObject deathPanel; // death panel with respawn button

    GameObject killTextNotificationGameObject; // a gameobject that I assign later in the script

    bool hasDeathPanelActivated = false;

    GameObject canvas;

    GameObject deathPanelGameObject;
    public GameObject musicHolder;

    public GameObject cinemachineCam;
    public GameObject virtualCam;

    GameObject cinemachineCamInstantiation;
    GameObject virtualCamInstantiation;

    public Player killer;


    private void Awake()
    {
        PV = GetComponent<PhotonView>();
    }

    // Start is called before the first frame update
    void Start()
    {
        if (PV.IsMine)
        {
            CreateController();
            killTextNotificationHolder = GameObject.Find("KillTextNotificationHolder");
            canvas = GameObject.Find("ScoreBoardCanvas");
            GameObject musicHolderGO = Instantiate(musicHolder);
            musicHolderGO.GetComponent<PersonalMusicManager>().PV = PV;
        }
    }

    private void Update()
    {
        if (!PV.IsMine)
            return;

        if (killTextNotificationGameObject == null && controller == null && hasDeathPanelActivated == false)
        {
            deathPanelGameObject = Instantiate(deathPanel, canvas.transform); // new gameobject called deathPanelGameObject with a prefab of deathPanel and the ui.transform

            //if the deathPanelGameObject isn't null
            if (deathPanelGameObject != null)
            {
                //has death panel activated is true
                hasDeathPanelActivated = true;
            }
            deathPanelGameObject.transform.Find("Replay").GetComponent<Button>().onClick.AddListener(Respawn); // of the deathPanelGameObject, find the button called "Replay" and add listener with the function called "Respawn"
            Cursor.lockState = CursorLockMode.None; // unlock the cursor
        }                
    }

    void CreateController()
    {        
        Transform spawnpoint = SpawnManager.Instance.GetSpawnPoint();
        controller = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerController"), spawnpoint.position, spawnpoint.rotation, 0, new object[] { PV.ViewID });
    }

    void Respawn()
    {
        Destroy(deathPanelGameObject);
        Destroy(virtualCamInstantiation);
        Destroy(cinemachineCamInstantiation);
        CreateController();
    }

    public void Die()
    {
        if (!PV.IsMine)
            return;

        if (controller.GetComponent<PlayerController>().hasDiedFromFallDamage == false)
        {
            EnableCinemachineKillerTracker();
        }

        PhotonNetwork.Destroy(controller);

        killTextNotificationGameObject = Instantiate(killTextNotification, killTextNotificationHolder.transform); // instantiate a new kill text notif
        killTextNotificationGameObject.GetComponent<TMP_Text>().text = "You were killed by: The Void"; // set the text of that to you were killed by the void
        Destroy(killTextNotificationGameObject, 5); // destroy that in 5 secs

        deaths++;

        Hashtable hash = new Hashtable();
        hash.Add("deaths", deaths);
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
    }

    void EnableCinemachineKillerTracker()
    {
        cinemachineCamInstantiation = Instantiate(cinemachineCam, this.transform.position, Quaternion.identity);
        GameObject virtualCamera = Instantiate(virtualCam, this.transform.position, Quaternion.identity);

        if (Find(killer).transform.gameObject != null)
        {
            virtualCamera.GetComponent<CinemachineVirtualCamera>().LookAt = Find(killer).transform;
        }

        virtualCamInstantiation = virtualCamera;
    }

    public void GetKill()
    {
        PV.RPC(nameof(RPC_GetKill), PV.Owner);
    }


    [PunRPC]
    void RPC_GetKill()
    {
        kills++;

        Hashtable hash = new Hashtable();
        hash.Add("kills", kills);
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
    }

    public static PlayerManager Find(Player player)
    {
        return FindObjectsOfType<PlayerManager>().SingleOrDefault(x => x.PV.Owner == player);
    }
}
