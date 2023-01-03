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
    public GameObject[] cape;

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
            PV.RPC(nameof(EquipEyewear), RpcTarget.All, aM.equipedEyewear);
            PV.RPC(nameof(EquipCape), RpcTarget.All, aM.equipedCape);
        }
            
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        //Call RPC with "newPlayer" as target
        if (PV.IsMine)
        {
            PV.RPC(nameof(EquipHat), newPlayer, aM.equipedHat);
            PV.RPC(nameof(EquipEyewear), newPlayer, aM.equipedEyewear);
            PV.RPC(nameof(EquipCape), newPlayer, aM.equipedCape);
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

    void EquipCape(int index)
    {
        for (int i = 0; i < cape.Length; i++)
        {
            if (i != index)
            {
                cape[i].SetActive(false);
            }
            else
            {
                cape[i].SetActive(true);
            }
        }
    }
}
