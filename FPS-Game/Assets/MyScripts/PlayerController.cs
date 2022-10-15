using System.Collections;
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
    [SerializeField] Image healthBarImage; // health bar image from Tutorial
    [SerializeField] GameObject ui; // ui from Tutorial
    
    [SerializeField] GameObject cameraHolder; // Camera holder from Tutorial
    public AudioClip killSFX; // kill SFX that plays to killer when victim dies

    public float mouseSensitivity, sprintSpeed, walkSpeed, jumpForce, smoothTime; // player variables from Tutorial

    [SerializeField] Item[] items; // gun items from Tutorial

    int itemIndex; // item index from Tutorial
    int previousItemIndex = -1; // previous item index from Tutorial

    float verticalLookRotation; // vertical look rotation from Tutorial
    bool grounded; // grounded bool from Tutorial
    Vector3 smoothMoveVelocity; // smooth move velocity from Tutorial
    Vector3 moveAmount; // move amount from Tutorial

    Rigidbody rb; // rigidbody

    public PhotonView PV; // photonview

    const float maxHealth = 100f; // max health from Tutorial
    public float currentHealth = maxHealth; // current health from Tutorial

    PlayerManager playerManager; // player manager

    [SerializeField] GameObject healthy;
    [HideInInspector] public int playerHealth;

    public GameObject redDeathParticleSystem;

    public bool isDead = false;

    [SerializeField] GameObject overheadUsernameText;
    [SerializeField] GameObject itemHolder;
    [SerializeField] GameObject healthBar;

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

    [SerializeField] GameObject gunClippingCam;

    public bool canSwitchWeapons = true;

    public GameObject scoreBoard;

    public GameObject[] canvas;

    bool canRegen = false;

    [SerializeField] GameObject playerHitParticleEffect;

    public Shader glowShader;

    bool micIsOn = true;

    GameObject micToggleText;

    public BuildingSystem buildingSystem;

    GameObject roomViewerCamera;

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
            Destroy(buildingSystem.handHeldBlock);
            Destroy(buildingSystem.blockCrosshair);
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
        if (PhotonNetwork.IsConnectedAndReady == false)
            return;

        if (!PV.IsMine)
            return;
        
        scoreBoard = GameObject.Find("ScoreBoard");
        micToggleText = GameObject.Find("MicToggleText");
        roomViewerCamera = GameObject.Find("RoomViewerCamera");

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
            GameObject deathPanelGameObject = Instantiate(deathPanel, ui.transform);
            if (deathPanelGameObject != null)
            {
                hasInstantiatedDeathPanel = true;
            }
            deathPanelGameObject.transform.Find("Replay").GetComponent<Button>().onClick.AddListener(Respawn);
            Cursor.lockState = CursorLockMode.None;
        }

        string _deviceName = Microphone.devices[0];

        if (Input.GetKeyDown(KeyCode.M)) 
        {
            micIsOn = !micIsOn;
            if (micIsOn)
            {
                Microphone.End(_deviceName);                
            }
            else
            {                
                Microphone.Start(_deviceName, true, 10, AudioSettings.outputSampleRate);
            }
        }
        if (micIsOn) micToggleText.GetComponent<TMP_Text>().text = "Click 'M' to toggle mic on";
        else micToggleText.GetComponent<TMP_Text>().text = "Click 'M' to toggle mic off";

        if (PV.IsMine)
            scoreBoard.GetComponent<ScoreBoard>().OpenLeaveConfirmation();

        if (scoreBoard.GetComponent<ScoreBoard>().areYouSureYouWantToLeaveConfirmation.alpha == 1)
        {
            StartCoroutine(nameof(Leave));
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
        if (PV.IsMine && PhotonNetwork.IsConnectedAndReady)
        {
            Hashtable hash = new Hashtable();
            if (hash.ContainsKey("healthColor"))
            {
                hash.Remove("healthColor");
            }
            hash.Add("healthColor", playerHealth);
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);

            PV.RPC(nameof(SetGlowIntensitity), RpcTarget.All);
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
            if (Input.GetAxisRaw("Mouse ScrollWheel") < 0f)
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
            if (Input.GetAxisRaw("Mouse ScrollWheel") > 0f)
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

        if (transform.position.y < -10f && PV.IsMine)
        {
            Die();
            killTextNotificationGameObject = Instantiate(killTextNotification, killTextNotificationHolder.transform);
            killTextNotificationGameObject.GetComponent<TMP_Text>().text = "You were killed by: The Void";
            Destroy(killTextNotificationGameObject, 5);            
        }
    }

    [PunRPC]
    void SetGlowIntensitity()
    {
        healthy.transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().material.SetFloat("_FresnelGlowIntensity", 2.5f);
        healthy.transform.GetChild(1).gameObject.GetComponent<MeshRenderer>().material.SetFloat("_FresnelGlowIntensity", 2.5f);
    }

    void Look()
    {
        transform.Rotate(Input.GetAxisRaw("Mouse X") * mouseSensitivity * Vector3.up);

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

    IEnumerator Leave()
    {
        if (PV.IsMine && PV.isActiveAndEnabled)
        {
            yield return new WaitForSeconds(0.01f);
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                StartCoroutine(DisconnectAndLoad());
            }
        }
    }

    IEnumerator DisconnectAndLoad()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            PhotonNetwork.LeaveRoom();
            Destroy(RoomManager.Instance.gameObject);

            normalCam.gameObject.SetActive(false);
            gunClippingCam.gameObject.SetActive(false);
            roomViewerCamera.SetActive(true);

            while (PhotonNetwork.InRoom)
                yield return null;

            SceneManager.LoadScene(0);
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
            if (hash.ContainsKey("itemIndex"))
            {
                hash.Remove("itemIndex");
            }
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

       if (changedProps.ContainsKey("beanColor") && !PV.IsMine && targetPlayer == PV.Owner)
       {
            Material healthyMat = new Material(glowShader);
            if (ColorUtility.TryParseHtmlString("#" + changedProps["beanColor"], out Color healthyColor))
            {
                healthyMat.SetColor("_MaterialColor", healthyColor);
                healthy.transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().material = healthyMat;
                healthy.transform.GetChild(1).gameObject.GetComponent<MeshRenderer>().material = healthyMat;
            }        
       }

       if (changedProps.ContainsKey("healthColor") && !PV.IsMine && targetPlayer == PV.Owner)
       {
            Debug.Log(playerHealth);
            if ((int)changedProps["healthColor"] == 3)
            {
                healthy.transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().material.SetColor("_FresnelColor", Color.green);
                healthy.transform.GetChild(1).gameObject.GetComponent<MeshRenderer>().material.SetColor("_FresnelColor", Color.green);
                Debug.Log("The health colour should change from green to GREEN");
            }
            else if ((int)changedProps["healthColor"] == 2)
            {
                healthy.transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().material.SetColor("_FresnelColor", Color.yellow);
                healthy.transform.GetChild(1).gameObject.GetComponent<MeshRenderer>().material.SetColor("_FresnelColor", Color.yellow);
                Debug.Log("The health colour should change from green to YELLOW");
            }
            else if((int)changedProps["healthColor"] == 1)
            {
                healthy.transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().material.SetColor("_FresnelColor", Color.red);
                healthy.transform.GetChild(1).gameObject.GetComponent<MeshRenderer>().material.SetColor("_FresnelColor", Color.red);
                Debug.Log("The health colour should change from green to RED");
            }
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
        } else
        {
            this.gameObject.GetComponent<AudioSource>().Stop();
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
            if (isDead || !PV.IsMine)
                return;

            PlayerManager.Find(info.Sender).GetKill();

            Debug.Log(info.Sender.NickName);

            PV.RPC(nameof(RPC_PlayKillDingSFX), info.Sender);

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

    public void SetPlayerHealthShader()
    {

        if (PV.IsMine)
        {
            Hashtable hash = new Hashtable();
            if (hash.ContainsKey("beanColor"))
            {
                hash.Remove("beanColor");
            }
            hash.Add("beanColor", PlayerPrefs.GetString("BeanPlayerColor"));
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
            if (playerHealth == 3)
                SetHealthyNewMaterial();

            if (playerHealth == 2)
                SetNormalNewMaterial();

            if (playerHealth == 1)
                SetHurtNewMaterial();
        }        
        
    }

    void SetHealthyNewMaterial()
    {
        Material healthyMat = new Material(glowShader);
        if (ColorUtility.TryParseHtmlString("#" + PlayerPrefs.GetString("BeanPlayerColor"), out Color healthyColor))
        {
            healthyMat.SetColor("_MaterialColor", healthyColor);
            healthyMat.SetColor("_FresnelColor", Color.green);
            healthy.transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().material = healthyMat;
            healthy.transform.GetChild(1).gameObject.GetComponent<MeshRenderer>().material = healthyMat;
        }
        
    }
    void SetNormalNewMaterial()
    {
        Material normalMat = new Material(glowShader);
        if (ColorUtility.TryParseHtmlString("#" + PlayerPrefs.GetString("BeanPlayerColor"), out Color normalColor))
        {
            normalMat.SetColor("_MaterialColor", normalColor);
            normalMat.SetColor("_FresnelColor", Color.yellow);
            healthy.transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().material = normalMat;
            healthy.transform.GetChild(1).gameObject.GetComponent<MeshRenderer>().material = normalMat;
        }
    }
    void SetHurtNewMaterial()
    {
        Material hurtMat = new Material(glowShader);
        if (ColorUtility.TryParseHtmlString("#" + PlayerPrefs.GetString("BeanPlayerColor"), out Color hurtColor))
        {
            hurtMat.SetColor("_MaterialColor", hurtColor);
            hurtMat.SetColor("_FresnelColor", Color.red);
            healthy.transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().material = hurtMat;
            healthy.transform.GetChild(1).gameObject.GetComponent<MeshRenderer>().material = hurtMat;
        }
    }

    void Die()
    {
        if (!PV.IsMine)
            return;

        PV.RPC(nameof(RPC_DisplayDeath), RpcTarget.All);
        Destroy(buildingSystem);
        GetComponent<CapsuleCollider>().enabled = false;

        for (int i = 0; i < canvas.Count(); i++)
        {
            Destroy(canvas[i]);
        }

        isDead = true;
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition;
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
        Destroy(itemHolder);
        Destroy(overheadUsernameText);
        Destroy(healthBar);
        GetComponent<CapsuleCollider>().enabled = false;      
    }
}