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

        DisplayUI();
        UseShield();
        ShieldSway();
    }

    private void UseShield()
    {
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
    }

    private void DisplayUI()
    {
        shieldBar.GetComponent<Image>().fillAmount = (shieldHealth / 20);
        glowFade.GetComponent<Image>().fillAmount = (shieldHealth / 20);
        shieldHealthText.text = (shieldHealth * 5).ToString();
    }

    private void ShieldSway()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * 2;
        float mouseY = Input.GetAxisRaw("Mouse Y") * 2;

        Quaternion rotationX = Quaternion.AngleAxis(-mouseY, Vector3.right);
        Quaternion rotationY = Quaternion.AngleAxis(mouseX, Vector3.up);

        Quaternion targetRotation = rotationX * rotationY;

        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, 8 * Time.deltaTime);
    }

    [PunRPC]
    void Open()
    {
        transform.GetChild(0).gameObject.SetActive(true);
        GetComponent<Animator>().Play("ShieldOpen");

        weapons[controller.itemIndex].transform.gameObject.GetComponent<SingleShotGun>().DisableGun();
        controller.walkSpeed = 2.5f;
        controller.sprintSpeed = 5;

        GetComponent<AudioSource>().PlayOneShot(equip);
    }

    [PunRPC]
    void Close()
    {
        GetComponent<Animator>().Play("ShieldClose");
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
            ChangeShieldDamageInitialVisual(6, 7);
        }
        if (shieldHealth <= 14 && shieldHealth >= 10)
        {
            ChangeShieldDamageVisual(7, 8);
        }
        if (shieldHealth <= 6 && shieldHealth >= 2)
        {
            ChangeShieldDamageVisual(8, 9);
        }
        if (shieldHealth <= 1 && shieldHealth >= 0)
        {
            ChangeShieldDamageVisual(9, 10);
        }
    }

    private void ChangeShieldDamageInitialVisual(int _old, int _new)
    {
        transform.GetChild(0).GetChild(_old).gameObject.GetComponent<MeshRenderer>().enabled = false;
        transform.GetChild(0).GetChild(_new).gameObject.SetActive(true);
    }

    private void ChangeShieldDamageVisual(int _old, int _new)
    {
        transform.GetChild(0).GetChild(_old).gameObject.SetActive(false);
        transform.GetChild(0).GetChild(_new).gameObject.SetActive(true);
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

        ResetShieldDamageVisual();

        lockShield = false;
    }

    private void ResetShieldDamageVisual()
    {
        transform.GetChild(0).GetChild(6).gameObject.GetComponent<MeshRenderer>().enabled = true;
        transform.GetChild(0).GetChild(7).gameObject.SetActive(false);
        transform.GetChild(0).GetChild(8).gameObject.SetActive(false);
        transform.GetChild(0).GetChild(9).gameObject.SetActive(false);
        transform.GetChild(0).GetChild(10).gameObject.SetActive(false);
    }
}
