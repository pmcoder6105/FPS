using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class DaggerController : MonoBehaviour
{
    public BoxCollider boxCollider;
    public PhotonView pV;
    public SingleShotGun daggerManager;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (daggerManager.canDaggerSwing == false && other.gameObject.transform != transform.root.gameObject && pV.IsMine)
        {
            other.gameObject.GetComponent<IDamageable>()?.TakeDamage(75);
        }
    }
}
