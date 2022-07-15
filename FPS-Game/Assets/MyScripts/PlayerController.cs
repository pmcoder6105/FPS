using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Photon.Realtime;
using TMPro;

public class PlayerController : MonoBehaviourPunCallbacks, IDamageable
{
    [SerializeField] Image healthBarImage;
    [SerializeField] GameObject ui;
    
    [SerializeField] GameObject cameraHolder;

    [SerializeField] float mouseSensitivity, sprintSpeed, walkSpeed, jumpForce, smoothTime;

    [SerializeField] Item[] items;

    int itemIndex;
    int previousItemIndex = -1;

    float verticalLookRotation;
    bool grounded;
    Vector3 smoothMoveVelocity;
    Vector3 moveAmount;

    Rigidbody rb;

    PhotonView PV;

    const float maxHealth = 100f;
    public float currentHealth = maxHealth;

    PlayerManager playerManager;

    [SerializeField] GameObject healthy;
    [SerializeField] GameObject normal;
    [SerializeField] GameObject hurt;
    int playerHealth;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        PV = GetComponent<PhotonView>();

        playerManager = PhotonView.Find((int)PV.InstantiationData[0]).GetComponent<PlayerManager>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Start()
    {
        if (PV.IsMine)
        {
            EquipItem(0);
        }
        else
        {
            Destroy(GetComponentInChildren<Camera>().gameObject);
            Destroy(rb);
            Destroy(ui);
        }
    }

    private void Update()
    {
        if (!PV.IsMine)
            return;

        Look();

        Move();

        Jump();

        SetPlayerHealthShader();

        for (int i = 0; i < items.Length; i++)
        {
            if (Input.GetKeyDown((i + 1).ToString()))
            {
                EquipItem(i);
                break;
            }
        }

        if (Input.GetAxisRaw("Mouse ScrollWheel") > 0f)
        {
            if (itemIndex >= items.Length - 1)
            {
                EquipItem(0);
            }
            else
            {
                EquipItem(itemIndex + 1);
            }
        }
        if (Input.GetAxisRaw("Mouse ScrollWheel") < 0f)
        {
            if (itemIndex <= 0)
            {
                EquipItem(items.Length - 1);
            }
            else
            {
                EquipItem(itemIndex - 1);
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            items[itemIndex].Use();
        }

        if (transform.position.y < -10f)
        {
            Die();
        }
    }

    void Look()
    {
        transform.Rotate(Vector3.up * Input.GetAxisRaw("Mouse X") * mouseSensitivity);

        verticalLookRotation += Input.GetAxisRaw("Mouse Y") * mouseSensitivity;
        verticalLookRotation = Mathf.Clamp(verticalLookRotation, -90f, 90f);

        cameraHolder.transform.localEulerAngles = Vector3.left * verticalLookRotation;
    }

    void Move()
    {
        Vector3 moveDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;

        moveAmount = Vector3.SmoothDamp(moveAmount, moveDir * (Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed), ref smoothMoveVelocity, smoothTime);
    }

    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && grounded)
        {
            rb.AddForce(transform.up * jumpForce);
        }
    }

    void EquipItem(int _index)
    {
        if (_index == previousItemIndex)
            return;

        itemIndex = _index;

        items[itemIndex].itemGameObject.SetActive(true);

        if (previousItemIndex != -1)
        {
            items[previousItemIndex].itemGameObject.SetActive(false);
        }

        previousItemIndex = itemIndex;

        if (PV.IsMine)
        {
            Hashtable hash = new Hashtable();
            hash.Add("itemIndex", itemIndex);
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        }
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
       if (changedProps.ContainsKey("itemIndex") && !PV.IsMine && targetPlayer == PV.Owner)
       {
            EquipItem((int)changedProps["itemIndex"]);
       }
    }

    public void SetGroundedState(bool _grounded)
    {
        grounded = _grounded;
    }

    private void FixedUpdate()
    {
        if (!PV.IsMine)
            return;
        rb.MovePosition(rb.position + transform.TransformDirection(moveAmount) * Time.fixedDeltaTime);
    }

    public void TakeDamage(float damage)
    {
        PV.RPC(nameof(RPC_TakeDamage), PV.Owner, damage);
    }

    [PunRPC]
    void RPC_TakeDamage(float damage, PhotonMessageInfo info)
    {
        currentHealth -= damage;

        healthBarImage.fillAmount = currentHealth / maxHealth;

        if (currentHealth <= 100 && currentHealth >= 50)
        {
            playerHealth = 3;
        }
        if (currentHealth <= 49 && currentHealth >= 30)
        {
            playerHealth = 2;
        }
        if (currentHealth <= 29 && currentHealth >= 0)
        {
            playerHealth = 1;
        }

        if (currentHealth <= 0)
        {
            Debug.Log("You were killed by " + info.Sender.NickName.ToString());
            Die();
            PlayerManager.Find(info.Sender).GetKill();
            Player player = info.Sender;
            PV.RPC(nameof(RPC_InstantiateKillText), player);
        }
    }

    [PunRPC]
    void RPC_InstantiateKillText()
    {
        Debug.Log("You got a kill!");
    }

    public void SetPlayerHealthShader()
    {
        if (PV.IsMine)
        {
            PV.RPC(nameof(RPC_SetPlayerHealthShader), RpcTarget.All, playerHealth);
        }        
    }

    [PunRPC]
    void RPC_SetPlayerHealthShader(int _playerHealth)
    {
        if (_playerHealth == 3)
        {
            healthy.SetActive(true);
            normal.SetActive(false);
            hurt.SetActive(false);
        }
        if (_playerHealth == 2)
        {
            healthy.SetActive(false);
            normal.SetActive(true);
            hurt.SetActive(false);
        }
        if (_playerHealth == 1)
        {
            healthy.SetActive(false);
            normal.SetActive(false);
            hurt.SetActive(true);
        }
    }

    void Die()
    {
        if (!PV.IsMine)
            return;
        playerManager.Die();
    }
}