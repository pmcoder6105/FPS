using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class EquipAccessories : MonoBehaviourPunCallbacks
{
    AccessoriesManager aM;
    public GameObject[] hats, eyeWear, cape;

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
        if (PV.IsMine)
        {
            accessoriesParent.SetActive(false);

            if (!AccessoriesManager.Singleton.removeHats)
            {
                PV.RPC(nameof(EquipHat), RpcTarget.AllBuffered, AccessoriesManager.Singleton.equipedHat);

            }

            if (!AccessoriesManager.Singleton.removeEyewear)
            {
                PV.RPC(nameof(EquipEyewear), RpcTarget.AllBuffered, AccessoriesManager.Singleton.equipedEyewear);

            }

            if (!AccessoriesManager.Singleton.removeCapes)
            {
                PV.RPC(nameof(EquipCape), RpcTarget.AllBuffered, AccessoriesManager.Singleton.equipedCape);

            }
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

    [PunRPC]
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

    [PunRPC]
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
