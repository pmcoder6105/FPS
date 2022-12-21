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

    public PlayerController controller;


    public GameObject[] weapons;

    public AudioClip equip;
    public AudioClip unequip;



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

        if (Input.GetKeyDown(KeyCode.Q) && canUseShield)
        {
            PV.RPC(nameof(Open), RpcTarget.All);
        }

        if (hasOpenedShield)
        {
            if (Input.GetKeyDown(KeyCode.Q) && canUseShield)
            {
                PV.RPC(nameof(Close), RpcTarget.All);

            }
        }

        Vector2 mouseAxis = new(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));

        transform.localPosition += (Vector3)mouseAxis * -5 / 1000;
    }

    [PunRPC]
    void Open()
    {
        transform.GetChild(0).gameObject.SetActive(true);
        GetComponent<Animator>().Play("ShieldOpen");
        //transform.root.GetComponent<PlayerController>().items[GetComponent<PlayerController>().itemIndex].itemGameObject.SetActive(false);
        //hasOpenedShield = true;
        //hasClosedShield = false;

        weapons[controller.itemIndex].transform.gameObject.GetComponent<SingleShotGun>().DisableGun();
        controller.walkSpeed /= 2;
        controller.sprintSpeed /= 2;

        GetComponent<AudioSource>().PlayOneShot(equip);
    }

    [PunRPC]
    void Close()
    {
        GetComponent<Animator>().Play("ShieldClose");
        //transform.root.GetComponent<PlayerController>().items[GetComponent<PlayerController>().itemIndex].itemGameObject.SetActive(true);
        //hasOpenedShield = false;
        //hasClosedShield = true;
        EnableGun();

        controller.walkSpeed = 5;
        controller.sprintSpeed = 10;
        GetComponent<AudioSource>().PlayOneShot(unequip);

    }

    void EnableGun()
    {
        int index = controller.itemIndex;
        controller.canSwitchWeapons = false;
        weapons[index].transform.GetChild(0).gameObject.SetActive(true);
        weapons[index].transform.GetComponent<SingleShotGun>().canAim = true;
        weapons[index].transform.GetComponent<SingleShotGun>()._canShoot = true;
        weapons[index].transform.GetComponent<SingleShotGun>().enabled = true;
    }
}
