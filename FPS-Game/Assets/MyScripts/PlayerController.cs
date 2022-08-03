using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Photon.Realtime;
using TMPro;
using Cinemachine;
using System.Linq;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviourPunCallbacks, IDamageable
{
    [SerializeField] Image healthBarImage;
    [SerializeField] GameObject ui;
    
    [SerializeField] GameObject cameraHolder;
    public AudioClip killSFX;

    public float mouseSensitivity, sprintSpeed, walkSpeed, jumpForce, smoothTime;

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

    public GameObject redDeathParticleSystem;

    public bool isDead = false;

    [SerializeField] GameObject overheadUsernameText;
    [SerializeField] GameObject itemHolder;
    [SerializeField] GameObject healthBar;

    //EntityManager entity;

    [SerializeField] Camera cinemachineCam;
    [SerializeField] Camera normalCam;
    [SerializeField] CinemachineVirtualCamera virtualCam;

    [SerializeField] GameObject killTextNotification;
    public GameObject killTextNotificationHolder;
    [SerializeField] GameObject deathPanel;

    GameObject killTextNotificationGameObject;

    bool hasInstantiatedDeathPanel = false;

    [SerializeField] GameObject publicKillTextNotification;
    public GameObject publicKillTextNotificationHolder;

    GameObject publicKillTextNotificationGameObject;

    [SerializeField] GameObject gunClippingCam;

    public bool canSwitchWeapons = true;

    public GameObject scoreBoard;

    public GameObject[] canvas;

    //public GameObject[] weapons;

    bool canRegen = false;

    [SerializeField] GameObject playerHitParticleEffect;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        PV = GetComponent<PhotonView>();

        playerManager = PhotonView.Find((int)PV.InstantiationData[0]).GetComponent<PlayerManager>();
        //entity = FindObjectOfType<EntityManager>();
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
            for (int i = 0; i < canvas.Count(); i++)
            {
                Destroy(canvas[i]);
            }
        }

        if (SceneManager.GetActiveScene().buildIndex == 6)
        {
            Physics.gravity = new Vector3(0, -2, 0);
            jumpForce = 500;
        }
    }

    private void Update()
    {
        if (!PV.IsMine)
            return;

        scoreBoard = GameObject.Find("ScoreBoard");

        if (Input.GetKeyDown(KeyCode.Escape) && Cursor.lockState == CursorLockMode.Locked && hasInstantiatedDeathPanel == false)
        {
            Cursor.lockState = CursorLockMode.None;
        }
        if (Input.GetMouseButtonDown(0) && Cursor.lockState == CursorLockMode.None && hasInstantiatedDeathPanel == false && scoreBoard.GetComponent<ScoreBoard>().isOpen == false && scoreBoard.GetComponent<ScoreBoard>().areYouSureYouWantToLeaveConfirmation.alpha != 1)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        if (killTextNotificationGameObject == null && isDead && hasInstantiatedDeathPanel == false)
        {
            Debug.Log("Time to spawn panel");
            GameObject deathPanelGameObject = Instantiate(deathPanel, ui.transform);
            if (deathPanelGameObject != null)
            {
                hasInstantiatedDeathPanel = true;
            }
            deathPanelGameObject.transform.Find("Replay").GetComponent<Button>().onClick.AddListener(Respawn);
            Debug.Log(deathPanelGameObject.transform.Find("Replay"));
            Cursor.lockState = CursorLockMode.None;
        }

        if (isDead == true)
            return;

        Look();

        Move();

        Jump();

        SetPlayerHealthShader();

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

        if (canSwitchWeapons)
        {
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
        }

        items[itemIndex].Use();

        if (transform.position.y < -10f)
        {
            Die();
            killTextNotificationGameObject = Instantiate(killTextNotification, killTextNotificationHolder.transform);
            killTextNotificationGameObject.GetComponent<TMP_Text>().text = "You were killed by: The Void";
            Destroy(killTextNotificationGameObject, 5);            
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
        PV.RPC(nameof(RPC_DisplayDamage), RpcTarget.All);
    }

    [PunRPC]
    public void RPC_DisplayDamage()
    {
        GameObject playerHitEffect = Instantiate(playerHitParticleEffect, this.gameObject.transform.position, Quaternion.identity);
        playerHitEffect.GetComponent<ParticleSystem>().Emit(15);
        Destroy(playerHitEffect, 2f);

    }

    [PunRPC]
    public void RPC_PlayKillDingSFX()
    {
        Debug.Log("Play ding sfx symbolizing a kill");
        if (this.gameObject.GetComponent<AudioSource>().isPlaying == false)
        {
            this.gameObject.GetComponent<AudioSource>().PlayOneShot(killSFX);
        }
    }

    [PunRPC]
    void RPC_TakeDamage(float damage, PhotonMessageInfo info)
    {
        currentHealth -= damage;

        CancelInvoke(nameof(StartRegen));
        Invoke(nameof(StartRegen), 5f);
        canRegen = false;

        



        if (currentHealth <= 0)
        {
            if (isDead)
                return;

            PlayerManager.Find(info.Sender).GetKill();

            PV.RPC(nameof(RPC_PlayKillDingSFX), info.Sender);

            //if (PV.IsMine)
            //{
            //    InstantiateKillText();
            //    Debug.Log("Instantiate +1 kill now!");
            //    publicKillTextNotificationGameObject.GetComponent<TMP_Text>().text = info.Sender.NickName.ToString() + " killed " + PV.Owner.NickName;
            //}

            killTextNotificationGameObject = Instantiate(killTextNotification, killTextNotificationHolder.transform);
            killTextNotificationGameObject.GetComponent<TMP_Text>().text = "You were killed by: " + info.Sender.NickName.ToString();
            Destroy(killTextNotificationGameObject, 5);

            cinemachineCam.gameObject.SetActive(true);
            normalCam.gameObject.SetActive(false);
            virtualCam.gameObject.SetActive(true);
            virtualCam.LookAt = PlayerManager.Find(info.Sender).transform;

            Die();
        }
    } 

    void StartRegen()
    {
        canRegen = true;
        StartCoroutine(nameof(Regen));
    }

    IEnumerator Regen()
    {
        while (currentHealth < 100 && canRegen)
        {            
            yield return new WaitForSeconds(0.1f);
            currentHealth += 0.5f;
            Debug.Log("Regen needed");

        }
    }

    //void InstantiateKillText()
    //{
    //    PV.RPC(nameof(RPC_InstantiateKillText), RpcTarget.All);
    //}
    
    //{PunRPC]
    //void RPC_InstantiateKillText()
    //{
    //    if (!PV.IsMine)
    //        return;
    //    Debug.Log("Instantiate +1 kill now!");
    //    publicKillTextNotificationGameObject = Instantiate(publicKillTextNotification, publicKillTextNotificationHolder.transform);       
    //    Destroy(publicKillTextNotificationGameObject, 3);
    //    //publicKillTextNotificationGameObject.GetComponent<TMP_Text>().text = info.Sender.NickName.ToString() + " killed " + PV.Owner.NickName;

    //}

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

        PV.RPC(nameof(RPC_DisplayDeath), RpcTarget.All);


        for (int i = 0; i < canvas.Count(); i++)
        {
            Destroy(canvas[i]);
        }

        isDead = true;
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition;

        //comment out after testing to make info panel and personal death notification texts


        //playerManager.Die();
        

        //comment out after testing to make info panel and personal death notification texts
    }

    void Respawn()
    {
        if (!PV.IsMine)
            return;

        playerManager.Die();
    }

    [PunRPC] 
    void RPC_DisplayDeath()
    {
        GameObject particleSystem = PhotonNetwork.Instantiate(nameof(redDeathParticleSystem), this.gameObject.transform.position, Quaternion.identity, 0);
        particleSystem.GetComponent<ParticleSystem>().Emit(30);
        Destroy(particleSystem, 5f);

        Destroy(healthy);
        Destroy(normal);
        Destroy(hurt);
        Destroy(itemHolder);
        Destroy(overheadUsernameText);
        Destroy(healthBar);
        GetComponent<CapsuleCollider>().enabled = false;      
    }
}