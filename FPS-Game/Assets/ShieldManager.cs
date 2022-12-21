using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ShieldManager : MonoBehaviour
{
    PhotonView PV;
    public bool canUseShield = true;
    public bool hasOpenedShield = false;
    public bool hasClosedShield = false;


    private void Awake()
    {
        PV = GetComponent<PhotonView>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (PV.IsMine == false)
            return;

        if (Input.GetKeyDown(KeyCode.RightShift) || Input.GetKeyDown(KeyCode.LeftShift) && canUseShield)
        {
            PV.RPC(nameof(Open), RpcTarget.All);
        }

        if (hasOpenedShield)
        {
            if (Input.GetKeyUp(KeyCode.RightShift) || Input.GetKeyUp(KeyCode.LeftShift) && canUseShield)
            {
                PV.RPC(nameof(Close), RpcTarget.All);

            }
        }

        
    }

    [PunRPC]
    void Open()
    {
        transform.GetChild(0).gameObject.SetActive(true);
        GetComponent<Animator>().Play("ShieldOpen");
        //transform.root.GetComponent<PlayerController>().items[GetComponent<PlayerController>().itemIndex].itemGameObject.SetActive(false);
        hasOpenedShield = true;
        //hasClosedShield = false;
        
    }

    [PunRPC]
    void Close()
    {
        GetComponent<Animator>().Play("ShieldClose");
        //transform.root.GetComponent<PlayerController>().items[GetComponent<PlayerController>().itemIndex].itemGameObject.SetActive(true);
        hasOpenedShield = false;
        //hasClosedShield = true;
    }
}
