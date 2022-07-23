using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;
using Photon.Realtime;
using System.Linq;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using TMPro;

public class PlayerManager : MonoBehaviour
{
    PhotonView PV;

    public GameObject controller;

    public AudioClip killSFX;

    int kills;
    int deaths;

    [SerializeField] GameObject killTextNotification;

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
        }
    }


    [PunRPC]
    public void PlayKillDingSFX()
    {
        if (PV.IsMine)
        {
            if (controller.GetComponent<AudioSource>().isPlaying == false)
            {
                controller.gameObject.GetComponent<AudioSource>().PlayOneShot(killSFX);
            }
        }           
    }

    private void Update()
    {
        if (!PV.IsMine)
            return;

        this.gameObject.transform.position = controller.transform.position;
    }

    void CreateController()
    {
        if (PlayerPrefs.GetInt("PillColor") == 1)
        {
            Transform spawnpoint = SpawnManager.Instance.GetSpawnPoint();
            //
            controller = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerController1"), spawnpoint.position, spawnpoint.rotation, 0, new object[] { PV.ViewID });
            //
        }
        if (PlayerPrefs.GetInt("PillColor") == 2)
        {
            Transform spawnpoint = SpawnManager.Instance.GetSpawnPoint();
            controller = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerController2"), spawnpoint.position, spawnpoint.rotation, 0, new object[] { PV.ViewID });
        }
        if (PlayerPrefs.GetInt("PillColor") == 3)
        {
            Transform spawnpoint = SpawnManager.Instance.GetSpawnPoint();
            controller = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerController3"), spawnpoint.position, spawnpoint.rotation, 0, new object[] { PV.ViewID });
        }
        if (PlayerPrefs.GetInt("PillColor") == 4)
        {
            Transform spawnpoint = SpawnManager.Instance.GetSpawnPoint();
            controller = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerController4"), spawnpoint.position, spawnpoint.rotation, 0, new object[] { PV.ViewID });
        }
    }

    public void Die()
    {
        PhotonNetwork.Destroy(controller);
        CreateController();

        deaths++;

        Hashtable hash = new Hashtable();
        hash.Add("deaths", deaths);
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
    }

    public void GetKill()
    {
        PV.RPC(nameof(RPC_GetKill), PV.Owner);

        //if (PV.IsMine == false)
        //    return;

        //GameObject killTextGameObject = Instantiate(killTextNotification, controller.GetComponent<PlayerController>().killTextNotificationHolder.transform);
        //killTextGameObject.GetComponent<TMP_Text>().text = "You got a kill!";
        //Destroy(killTextGameObject, 3);
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
