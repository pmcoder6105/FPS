using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SingleShotGun : Gun
{
    [SerializeField] Camera cam;

    public GameObject bulletImpactVFX;

    PhotonView PV;

    public int playerID;

    [Header("Gun Settings")]

    public float fireRate = 0.1f;
    public int clipSize = 30;
    public int reservedAmmoCapacity = 270;
    public bool isAutomatic;

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

    void Shoot()
    {
        DetermineAim();
        DetermineWeaponSway();
        if (isAutomatic)
        {
            if (Input.GetMouseButton(0) && _canShoot && _currentAmmoInClip > 0)
            {
                _canShoot = false;
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
                int ammoNeeded = clipSize - _currentAmmoInClip;
                if (ammoNeeded >= _ammoInReserve)
                {
                    _currentAmmoInClip += _ammoInReserve;
                    _ammoInReserve = 0;
                } 
                else
                {
                    _currentAmmoInClip = clipSize;
                    _ammoInReserve -= ammoNeeded;
                }
            }
        } else
        {
            if (Input.GetMouseButtonDown(0) && _canShoot && _currentAmmoInClip > 0)
            {
                _canShoot = false;
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
                int ammoNeeded = clipSize - _currentAmmoInClip;
                if (ammoNeeded >= _ammoInReserve)
                {
                    _currentAmmoInClip += _ammoInReserve;
                    _ammoInReserve = 0;
                }
                else
                {
                    _currentAmmoInClip = clipSize;
                    _ammoInReserve -= ammoNeeded;
                }
            }
        }
        
    }

    void DetermineWeaponSway()
    {
        Vector2 mouseAxis = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));

        transform.localPosition += (Vector3)mouseAxis * weaponSwayAmount / 1000;
    }

    void DetermineAim()
    {
        Vector3 target = normalLocalPosition;
        if (Input.GetMouseButton(1)) target = aimingLocalPosition;

        Vector3 desiredPosition = Vector3.Lerp(transform.localPosition, target, Time.deltaTime * aimSmoothing);

        transform.localPosition = desiredPosition;
    }
    IEnumerator ShootGun()
    {
        GameObject flash = Instantiate(muzzleFlashEffect, muzzleFlashSpawnPlace);
        flash.GetComponent<ParticleSystem>().Emit(1);
        flash.transform.Find("Sparks").GetComponent<ParticleSystem>().Emit(1);
        DetermineRecoil();
        yield return new WaitForSeconds(fireRate);
        _canShoot = true;
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
            Destroy(impactObject, 3f);
            impactObject.transform.SetParent(colliders[0].transform);
        }
        
    }
}
