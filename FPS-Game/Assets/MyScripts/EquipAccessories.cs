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

            if (!aM.removeHats)
                PV.RPC(nameof(EquipHat), RpcTarget.AllBuffered, aM.equipedHat);

            if (!aM.removeEyewear)
                PV.RPC(nameof(EquipEyewear), RpcTarget.AllBuffered, aM.equipedEyewear);

            if (!aM.removeCapes)
                PV.RPC(nameof(EquipCape), RpcTarget.AllBuffered, aM.equipedCape);
        }
            
    }

    [PunRPC]
    void EquipHat(int index)
    {
        for (int i = 0; i < hats.Length; i++)
        {
            if (i - 1 != index)
            {
                hats[i].SetActive(false);
            }
            else
            {
                if (i != 0)
                {
                    hats[i].SetActive(true);
                }
            }
        }
    }

    [PunRPC]
    void EquipEyewear(int index)
    {
        for (int i = 0; i < eyeWear.Length; i++)
        {
            if (i - 1 != index)
            {
                eyeWear[i].SetActive(false);
            }
            else
            {
                if (i != 0)
                {
                    eyeWear[i].SetActive(true);
                }
            }
        }
    }

    [PunRPC]
    void EquipCape(int index)
    {
        for (int i = 0; i < cape.Length; i++)
        {
            if (i - 1 != index)
            {
                cape[i].SetActive(false);
            }
            else
            {
                if (i != 0)
                {
                    cape[i].SetActive(true);
                }
            }
        }
    }
}
