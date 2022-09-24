using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using System.Linq;

public class CullGuns : MonoBehaviour
{
    PhotonView PV;

    public GameObject[] weapons;

    private void Awake()
    {
        PV = GetComponent<PhotonView>();
    }

    private void OnPreCull()
    {
        if (PV.IsMine == false)
            return;

        this.gameObject.GetComponent<Camera>().cullingMask = 7;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (PV.IsMine)
        {
            Debug.Log("Should set all weapons' layer to Weapon");
            for (int i = 0; i < weapons.Count(); i++)
            {
                weapons[i].layer = LayerMask.NameToLayer("Weapon");
                SetLayerRecursively(weapons[i], 7);
            }
        }
    }

    public static void SetLayerRecursively(GameObject weapon, int layerNumber)
    {
        foreach (Transform trans in weapon.GetComponentsInChildren<Transform>(true))
        {
            trans.gameObject.layer = layerNumber;
        }
    }
}
