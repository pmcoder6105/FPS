using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;
using Photon.Realtime;
using System.Linq;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PlayerManager : MonoBehaviour
{
    PhotonView PV;

    GameObject controller;

    int kills;
    int deaths;

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

    // Update is called once per frame
    void Update()
    {

    }

    void CreateController()
    {
        if (PlayerPrefs.GetInt("PillColor") == 1)
        {
            Transform spawnpoint = SpawnManager.Instance.GetSpawnPoint();
            controller = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerController1"), spawnpoint.position, spawnpoint.rotation, 0, new object[] { PV.ViewID });
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
    }

    [PunRPC]
    void RPC_GetKill()
    {
        kills++;

        Debug.Log("You got a kill!");

        Hashtable hash = new Hashtable();
        hash.Add("kills", kills);
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
    }

    public static PlayerManager Find(Player player)
    {
        return FindObjectsOfType<PlayerManager>().SingleOrDefault(x => x.PV.Owner == player);
    }
}
