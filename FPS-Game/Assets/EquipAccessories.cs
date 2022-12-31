using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class EquipAccessories : MonoBehaviourPunCallbacks
{
    AccessoriesManager aM;
    public GameObject[] hats;
    public GameObject[] eyeWear;

    PhotonView PV;

    [SerializeField] GameObject accessoriesParent;


    private void Awake()
    {
        aM = GameObject.Find("AccessoriesManager").GetComponent<AccessoriesManager>();
        PV = GetComponent<PhotonView>();
    }

    // Start is called before the first frame update
    void Start()
    {


        //Call RPC with "all" as target
        if (PV.IsMine)
        {
            accessoriesParent.SetActive(false);
            PV.RPC(nameof(EquipHat), RpcTarget.All, aM.equipedHat);
        }
            
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        //Call RPC with "newPlayer" as target
        if (PV.IsMine)
        {
            PV.RPC(nameof(EquipHat), newPlayer, aM.equipedHat);
        }
    }


    [PunRPC]
    void EquipHat(int index)
    {
        for (int i = 0; i < hats.Length; i++)
        {
            if (i != index)
            {
                hats[i].SetActive(false);
            }
            else
            {
                hats[i].SetActive(true);
            }
        }
    }

    void EquipEyewear(int index)
    {
        for (int i = 0; i < eyeWear.Length; i++)
        {
            if (i != index)
            {
                eyeWear[i].SetActive(false);
            }
            else
            {
                eyeWear[i].SetActive(true);
            }
        }
    }
}
