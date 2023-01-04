using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using RayFire;
using UnityEngine.UI;

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

    public int shieldHealth = 20;

    bool lockShield = false;

    public GameObject shieldBar;

    public TMP_Text shieldHealthText;
    public GameObject glowFade;

    private void Awake()
    {
        PV = GetComponent<PhotonView>();
    }

    void Update()
    {
        if (!PV.IsMine)
            return;

        shieldBar.GetComponent<Image>().fillAmount = (shieldHealth / 20);
        glowFade.GetComponent<Image>().fillAmount = (shieldHealth / 20);


        shieldHealthText.text = (shieldHealth * 5).ToString();

        if (lockShield == false)
        {
            if (Input.GetKeyDown(KeyCode.Q) && canUseShield)
            {
                PV.RPC(nameof(Open), RpcTarget.All);
                shieldBar.SetActive(true);
            }

            if (hasOpenedShield)
            {
                if (Input.GetKeyDown(KeyCode.Q) && canUseShield)
                {
                    PV.RPC(nameof(Close), RpcTarget.All);
                    shieldBar.SetActive(false);


                }
            }
        }



        float mouseX = Input.GetAxisRaw("Mouse X") * 2;
        float mouseY = Input.GetAxisRaw("Mouse Y") * 2;

        // calculate target rotation
        Quaternion rotationX = Quaternion.AngleAxis(-mouseY, Vector3.right);
        Quaternion rotationY = Quaternion.AngleAxis(mouseX, Vector3.up);

        Quaternion targetRotation = rotationX * rotationY;

        // rotate 
        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, 8 * Time.deltaTime);
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
        controller.canSwitchWeapons = true;
        weapons[index].transform.GetChild(0).gameObject.SetActive(true);
        weapons[index].transform.GetComponent<SingleShotGun>().canAim = true;
        weapons[index].transform.GetComponent<SingleShotGun>()._canShoot = true;
        weapons[index].transform.GetComponent<SingleShotGun>().enabled = true;

        if (controller.inventoryEnabled)
        {
            transform.GetChild(1).gameObject.SetActive(true);
        }
    }

    public void TakeHit()
    {
        PV.RPC(nameof(RPC_TakeHit), PV.Owner);
        PV.RPC(nameof(RPC_VisualizeShieldDamage), RpcTarget.All);
    }

    [PunRPC]
    public void RPC_TakeHit()
    {
        shieldHealth--;

        if (shieldHealth <= 0)
        {
            StartCoroutine(nameof(WaitUntilPlayCanReopenShield));
        }
    }

    [PunRPC]
    void RPC_VisualizeShieldDamage()
    {
        if (shieldHealth <= 19 && shieldHealth >= 15)
        {
            transform.GetChild(0).GetChild(6).gameObject.SetActive(false);
            transform.GetChild(0).GetChild(7).gameObject.SetActive(true);
        }
        if (shieldHealth <= 14 && shieldHealth >= 10)
        {
            transform.GetChild(0).GetChild(7).gameObject.SetActive(false);
            transform.GetChild(0).GetChild(8).gameObject.SetActive(true);
        }
        if (shieldHealth <= 6 && shieldHealth >= 2)
        {
            transform.GetChild(0).GetChild(8).gameObject.SetActive(false);
            transform.GetChild(0).GetChild(9).gameObject.SetActive(true);
        }
        if (shieldHealth <= 1 && shieldHealth >= 0)
        {
            transform.GetChild(0).GetChild(9).gameObject.SetActive(false);
            transform.GetChild(0).GetChild(10).gameObject.SetActive(true);
        }
    }

    IEnumerator WaitUntilPlayCanReopenShield()
    {
        Close();
        canUseShield = false;

        yield return new WaitForSeconds(0.4f);

        lockShield = true;
        shieldBar.SetActive(true);
        shieldHealthText.text = "Shield Fixing...";

        yield return new WaitForSeconds(6.6f);

        transform.GetChild(0).GetChild(6).gameObject.SetActive(true);
        transform.GetChild(0).GetChild(7).gameObject.SetActive(false);
        transform.GetChild(0).GetChild(8).gameObject.SetActive(false);
        transform.GetChild(0).GetChild(9).gameObject.SetActive(false);
        transform.GetChild(0).GetChild(10).gameObject.SetActive(false);

        lockShield = false;
    }
}
