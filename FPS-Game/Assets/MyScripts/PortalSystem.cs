using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PortalSystem : MonoBehaviour
{
    [SerializeField] Transform tie;
    public AudioClip portalSound;

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.gameObject.GetComponent<PhotonView>() == null)
            return;

        //other.gameObject.GetComponent<AudioSource>().Stop();
        other.gameObject.GetComponent<AudioSource>().PlayOneShot(portalSound);
        other.gameObject.transform.root.transform.GetChild(0).GetChild(0).GetComponent<Animator>().Play("CameraWarpAnimation");

        this.gameObject.GetComponent<BoxCollider>().enabled = false;
        tie.gameObject.GetComponent<BoxCollider>().enabled = false;


        StartCoroutine(nameof(Warp), other);
    }

    private IEnumerator Warp(Collider other)
    {
        yield return new WaitForSeconds(0.2f);

        other.transform.SetPositionAndRotation(new Vector3(tie.position.x, tie.position.y - 0.3f, tie.position.z), tie.rotation);

        yield return new WaitForSeconds(.8f);

        this.gameObject.GetComponent<BoxCollider>().enabled = true;
        tie.gameObject.GetComponent<BoxCollider>().enabled = true;

        other.gameObject.transform.root.transform.GetChild(0).GetChild(0).GetComponent<Animator>().Play("CameraIdle");
    }

}
