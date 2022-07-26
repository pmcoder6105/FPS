using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using System.Linq;
using TMPro;

public class SingleShotGun : Gun
{
    [SerializeField] Camera cam;

    public GameObject bulletImpactVFX;
    public GameObject[] scopePieces;

    PhotonView PV;

    public int playerID;

    [Header("Gun Settings")]

    public float fireRate = 0.1f;
    public int clipSize = 30;
    public int reservedAmmoCapacity = 270;
    public bool isAutomatic;
    public float reloadTime;

    //Variables that change throughout the code
    bool _canShoot;
    int _currentAmmoInClip;
    int _ammoInReserve;

    //Muzzle Flash
    public GameObject muzzleFlashEffect;
    public Transform muzzleFlashSpawnPlace;

    //Aiming
    public Vector3 normalLocalPosition;
    public Vector3 aimingLocalPosition;
    public float aimSmoothing = 10;

    //Weapon Sway
    public float weaponSwayAmount = 10;

    //Weapon Recoil
    public bool randomizeRecoil;
    public Vector2 randomRecoilContstraints;
    //you only need to assign this is randomizerecoil is off
    public Vector2 recoilPattern;
    public float recoilAmount = 0.1f;

    [Header("Animations")]
    //Reloading
    public Animator animator;
    public string reload;
    public bool doesHaveAnimationForShooting;
    public string shoot;
    bool isReloading = false;
    public AudioSource audioSource;
    public AudioClip reloadSFX;
    public AudioClip outOfAmmoSFX;
    public bool isPistol;
    public string pistolOutOfAmmo;

    public PlayerController playerController;

    public bool isSniper;
    bool isScoped = false;
    public bool isShotGun;
    public GameObject scope;

    public Vector2 _currentRotation;
    bool canAim = true;

    public float shotGunRange;

    //Gun UI
    public TMP_Text bulletCount;

    public AudioClip[] shootSFX;

    public AudioClip equip;

    bool canShotgunShoot = false;

    public AudioClip scopeSFX;


    private void Start()
    {
        _currentAmmoInClip = clipSize;
        _ammoInReserve = reservedAmmoCapacity;
        _canShoot = true;
    }

    private void Awake()
    {
        PV = GetComponent<PhotonView>();        
    }

    private void Update()
    {        
    }

    public override void Use()
    {
        if (PV.IsMine == false)
            return;

        Shoot();
        
    }

    IEnumerator WaitUntilShotgunCanShoot()
    {
        yield return new WaitForSeconds(0.4f / 0.6f);
        canShotgunShoot = true;
    }

    [PunRPC]
    void PlayShootSFX()
    {
        int clipToPlay = Random.Range(0, shootSFX.Length - 1);
        audioSource.PlayOneShot(shootSFX[clipToPlay]);
    }

    void Shoot()
    {
        DetermineAim();
        DetermineWeaponSway();

        bulletCount.text = _currentAmmoInClip + " / " + clipSize;

        if (isAutomatic)
        {
            if (Input.GetMouseButton(0) && _canShoot && _currentAmmoInClip > 0)
            {
                PV.RPC(nameof(PlayShootSFX), RpcTarget.All);
                _canShoot = false;
                playerController.canSwitchWeapons = false;
                _currentAmmoInClip--;
                StartCoroutine(ShootGun());
                Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f));
                ray.origin = cam.transform.position;
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    hit.collider.gameObject.GetComponent<IDamageable>()?.TakeDamage(((GunInfo)itemInfo).damage);
                    PV.RPC("RPC_Shoot", RpcTarget.All, hit.point, hit.normal);
                }
                if (doesHaveAnimationForShooting)
                {
                    animator.Play(shoot.ToString(), 0, 0.0f);
                }
            }
            else if (Input.GetKeyDown(KeyCode.R) && _currentAmmoInClip < clipSize && _ammoInReserve > 0)
            {
                if (_canShoot == false)
                    return;

                if (isReloading == false)
                {
                    playerController.canSwitchWeapons = false;

                    StartCoroutine(Reload());
                    audioSource.Stop();
                    audioSource.PlayOneShot(reloadSFX);
                }                
            }
        } else
        {
            if (Input.GetMouseButtonDown(0) && _canShoot && _currentAmmoInClip > 0)
            {
                PV.RPC(nameof(PlayShootSFX), RpcTarget.All);
                _canShoot = false;
                playerController.canSwitchWeapons = false;
                _currentAmmoInClip--;
                StartCoroutine(ShootGun());
                if (isShotGun == false)
                {
                    Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f));
                    ray.origin = cam.transform.position;

                    if (Physics.Raycast(ray, out RaycastHit hit))
                    {
                        hit.collider.gameObject.GetComponent<IDamageable>()?.TakeDamage(((GunInfo)itemInfo).damage);
                        PV.RPC("RPC_Shoot", RpcTarget.All, hit.point, hit.normal);
                    }
                } else if (isShotGun)
                {
                    float x = Random.Range(-0.1f, 0.1f);
                    float y = Random.Range(-0.1f, 0.1f);
                    float x1 = Random.Range(-0.1f, 0.1f);
                    float y1 = Random.Range(-0.1f, 0.1f);
                    float x2 = Random.Range(-0.1f, 0.1f);
                    float y2 = Random.Range(-0.1f, 0.1f);
                    float x3 = Random.Range(-0.1f, 0.1f);
                    float y3 = Random.Range(-0.1f, 0.1f);
                    float x4 = Random.Range(-0.1f, 0.1f);
                    float y4 = Random.Range(-0.1f, 0.1f);
                    float x5 = Random.Range(-0.1f, 0.1f);
                    float y5 = Random.Range(-0.1f, 0.1f);
                    float x6 = Random.Range(-0.1f, 0.1f);
                    float y6 = Random.Range(-0.1f, 0.1f);
                    float x7 = Random.Range(-0.1f, 0.1f);
                    float y7 = Random.Range(-0.1f, 0.1f);

                    Vector3 directionOfRay = cam.transform.forward + new Vector3(x, y, 0);
                    Vector3 directionOfRay1 = cam.transform.forward + new Vector3(x1, y1, 0);
                    Vector3 directionOfRay2 = cam.transform.forward + new Vector3(x2, y2, 0);
                    Vector3 directionOfRay3 = cam.transform.forward + new Vector3(x3, y3, 0);
                    Vector3 directionOfRay4 = cam.transform.forward + new Vector3(x4, y4, 0);
                    Vector3 directionOfRay5 = cam.transform.forward + new Vector3(x5, y5, 0);
                    Vector3 directionOfRay6 = cam.transform.forward + new Vector3(x6, y6, 0);
                    Vector3 directionOfRay7 = cam.transform.forward + new Vector3(x7, y7, 0);

                    if (Physics.Raycast(cam.transform.position, directionOfRay, out RaycastHit hit, shotGunRange))
                    {
                        hit.collider.gameObject.GetComponent<IDamageable>()?.TakeDamage(((GunInfo)itemInfo).damage);
                        PV.RPC("RPC_Shoot", RpcTarget.All, hit.point, hit.normal);
                    }
                    if (Physics.Raycast(cam.transform.position, directionOfRay1, out RaycastHit hit1, shotGunRange))
                    {
                        hit1.collider.gameObject.GetComponent<IDamageable>()?.TakeDamage(((GunInfo)itemInfo).damage);
                        PV.RPC("RPC_Shoot", RpcTarget.All, hit1.point, hit1.normal);
                    }
                    if (Physics.Raycast(cam.transform.position, directionOfRay2, out RaycastHit hit2, shotGunRange))
                    {
                        hit2.collider.gameObject.GetComponent<IDamageable>()?.TakeDamage(((GunInfo)itemInfo).damage);
                        PV.RPC("RPC_Shoot", RpcTarget.All, hit2.point, hit2.normal);
                    }
                    if (Physics.Raycast(cam.transform.position, directionOfRay3, out RaycastHit hit3, shotGunRange))
                    {
                        hit3.collider.gameObject.GetComponent<IDamageable>()?.TakeDamage(((GunInfo)itemInfo).damage);
                        PV.RPC("RPC_Shoot", RpcTarget.All, hit3.point, hit3.normal);
                    }
                    if (Physics.Raycast(cam.transform.position, directionOfRay4, out RaycastHit hit4, shotGunRange))
                    {
                        hit4.collider.gameObject.GetComponent<IDamageable>()?.TakeDamage(((GunInfo)itemInfo).damage);
                        PV.RPC("RPC_Shoot", RpcTarget.All, hit4.point, hit4.normal);
                    }
                    if (Physics.Raycast(cam.transform.position, directionOfRay5, out RaycastHit hit5, shotGunRange))
                    {
                        hit5.collider.gameObject.GetComponent<IDamageable>()?.TakeDamage(((GunInfo)itemInfo).damage);
                        PV.RPC("RPC_Shoot", RpcTarget.All, hit5.point, hit5.normal);
                    }
                    if (Physics.Raycast(cam.transform.position, directionOfRay6, out RaycastHit hit6, shotGunRange))
                    {
                        hit6.collider.gameObject.GetComponent<IDamageable>()?.TakeDamage(((GunInfo)itemInfo).damage);
                        PV.RPC("RPC_Shoot", RpcTarget.All, hit6.point, hit6.normal);
                    }
                    if (Physics.Raycast(cam.transform.position, directionOfRay7, out RaycastHit hit7, shotGunRange))
                    {
                        hit7.collider.gameObject.GetComponent<IDamageable>()?.TakeDamage(((GunInfo)itemInfo).damage);
                        PV.RPC("RPC_Shoot", RpcTarget.All, hit7.point, hit7.normal);
                    }
                }                
                if (doesHaveAnimationForShooting)
                {
                    animator.Play(shoot.ToString(), 0, 0.0f);
                }
            }            
            else if (Input.GetKeyDown(KeyCode.R) && _currentAmmoInClip < clipSize && _ammoInReserve > 0)
            {
                if (_canShoot == false)
                    return;

                if (isReloading == false)
                {
                    playerController.canSwitchWeapons = false;

                    StartCoroutine(Reload());

                    audioSource.Stop();
                    audioSource.PlayOneShot(reloadSFX);
                }
            }
        }

        if (Input.GetMouseButtonDown(0) && _canShoot)
        {
            if (!isSniper)
            {
                if (_currentAmmoInClip <= 0)
                {
                    if (isPistol)
                    {
                        animator.Play(pistolOutOfAmmo);
                    }

                    audioSource.Stop();
                    audioSource.PlayOneShot(outOfAmmoSFX);
                }
            } else
            {
                if (_currentAmmoInClip <0)
                {
                    audioSource.Stop();
                    audioSource.PlayOneShot(outOfAmmoSFX);
                }
            }
            
        }
        
    }

    IEnumerator Reload()
    {
        animator.Play(reload.ToString(), 0, 0.0f);
        canAim = false;
        isReloading = true;
        _canShoot = false;
        yield return new WaitForSeconds(reloadTime);
        canAim = true;
        isReloading = false;
        _canShoot = true;
        playerController.canSwitchWeapons = true;

        _currentAmmoInClip = clipSize;
    }

    void DetermineWeaponSway()
    {
        Vector2 mouseAxis = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));

        transform.localPosition += (Vector3)mouseAxis * weaponSwayAmount / 1000;
    }

    void DetermineAim()
    {
        if (canAim == false)
            return;

        Vector3 target = normalLocalPosition;
        if (Input.GetMouseButton(1))
        {
            target = aimingLocalPosition;            
        }

        Debug.Log("Mouse Sensitivity is: " + playerController.mouseSensitivity);

        if (isSniper)
        {
            if (Input.GetMouseButtonDown(1))
            {
                isScoped = true;
                audioSource.Stop();
                audioSource.PlayOneShot(scopeSFX);
                StartCoroutine(OpenScope());
                playerController.mouseSensitivity = 0.5f;
            }
            else if (Input.GetMouseButtonUp(1))
            {
                isScoped = false;
                cam.fieldOfView = 60;
                playerController.mouseSensitivity = 3;
                for (int i = 0; i < scopePieces.Count(); i++)
                {
                    scopePieces[i].SetActive(true);
                }
                //weaponCam.SetActive(true);
                scope.SetActive(false);
                //Enable sniper visuals
                //Modify muzzleFlash
            }
        }       

        Vector3 desiredPosition = Vector3.Lerp(transform.localPosition, target, Time.deltaTime * aimSmoothing);

        transform.localPosition = desiredPosition;
    }

    IEnumerator OpenScope()
    {
        yield return new WaitForSeconds(Time.deltaTime * aimSmoothing);
        scope.SetActive(true);
        cam.fieldOfView = 30;
        for (int i = 0; i < scopePieces.Count(); i++)
        {
            scopePieces[i].SetActive(false);
        }
        //weaponCam.SetActive(false);
        //Disable sniper visuals
        //Modify muzzle flash
    }
    IEnumerator ShootGun()
    {
        GameObject flash = Instantiate(muzzleFlashEffect, muzzleFlashSpawnPlace);
        flash.GetComponent<ParticleSystem>().Emit(1);
        flash.transform.Find("Sparks").GetComponent<ParticleSystem>().Emit(1);
        DetermineRecoil();
        yield return new WaitForSeconds(fireRate);
        _canShoot = true;
        playerController.canSwitchWeapons = true;
    }

    void DetermineRecoil()
    {
        transform.localPosition -= Vector3.forward * recoilAmount;
    }

    [PunRPC]
    void RPC_Shoot(Vector3 hitPosition, Vector3 hitNormal)
    {
        Collider[] colliders = Physics.OverlapSphere(hitPosition, 0.3f);
        if (colliders.Length != 0)
        {
            GameObject impactObject = Instantiate(bulletImpactVFX, hitPosition, Quaternion.LookRotation(hitNormal));
            if (isScoped)
                impactObject.transform.localScale = impactObject.transform.localScale * 4;
            Destroy(impactObject, 3f);
            impactObject.transform.SetParent(colliders[0].transform);
        }
        
    }
}
