using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class DaggerController : MonoBehaviour
{
    public BoxCollider boxCollider;
    public PhotonView pV;
    public SingleShotGun daggerManager;

    private void OnTriggerEnter(Collider other)
    {
        if (daggerManager.canDaggerSwing == false && other.gameObject.transform != transform.root.gameObject && pV.IsMine)
        {
            other.gameObject.GetComponent<IDamageable>()?.TakeDamage(40);
            StartCoroutine(nameof(DisableDagger));
        }
    }

    private IEnumerator DisableDagger()
    {
        boxCollider.enabled = false;
        yield return new WaitForSeconds(2);
        boxCollider.enabled = true;
    }
}
