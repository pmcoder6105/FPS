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

    IEnumerator ShootGun()
    {
        GameObject flash = Instantiate(muzzleFlashEffect, muzzleFlashSpawnPlace);
        yield return new WaitForSeconds(fireRate);
        _canShoot = true;
        Destroy(flash);
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
