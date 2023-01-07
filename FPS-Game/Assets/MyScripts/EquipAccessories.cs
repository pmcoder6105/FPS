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

            if (!aM.removeHats)
                PV.RPC(nameof(EquipHat), RpcTarget.All, aM.equipedHat);

            if (!aM.removeEyewear)
                PV.RPC(nameof(EquipEyewear), RpcTarget.All, aM.equipedEyewear);

            if (!aM.removeCapes)
                PV.RPC(nameof(EquipCape), RpcTarget.All, aM.equipedCape);
        }
            
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        //Call RPC with "newPlayer" as target
        if (PV.IsMine)
        {
            accessoriesParent.SetActive(false);

            if (!aM.removeHats)
                PV.RPC(nameof(EquipHat), newPlayer, aM.equipedHat);

            if (!aM.removeEyewear)
                PV.RPC(nameof(EquipEyewear), newPlayer, aM.equipedEyewear);

            if (!aM.removeCapes)
                PV.RPC(nameof(EquipCape), newPlayer, aM.equipedCape);
        }
    }


    [PunRPC]
    void EquipHat(int index)
    {
        for (int i = 0; i < hats.Length; i++)
        {
            if (i + 1 != index)
            {
                hats[i + 1].SetActive(false);
            }
            else
            {
                hats[i + 1].SetActive(true);
            }
        }
    }

    [PunRPC]
    void EquipEyewear(int index)
    {
        for (int i = 0; i < eyeWear.Length; i++)
        {
            if (i + 1 != index)
            {
                eyeWear[i + 1].SetActive(false);
            }
            else
            {
                eyeWear[i + 1].SetActive(true);
            }
        }
    }

    [PunRPC]
    void EquipCape(int index)
    {
        for (int i = 0; i < cape.Length; i++)
        {
            if (i + 1 != index)
            {
                cape[i + 1].SetActive(false);
            }
            else
            {
                cape[i + 1].SetActive(true);
            }
        }
    }
}
