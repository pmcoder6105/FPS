using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PortalSystem : MonoBehaviour
{

    [SerializeField] Transform tie;
    public AudioClip portalSound;

    // Start is called before the first frame update
    void Start()
    {
        //tie = this.transform;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.gameObject.GetComponent<PhotonView>() == null)
            return;

        this.gameObject.GetComponent<BoxCollider>().enabled = false;
        tie.gameObject.GetComponent<BoxCollider>().enabled = false;

        other.transform.SetPositionAndRotation(new Vector3(tie.position.x, tie.position.y - 0.3f, tie.position.z), tie.rotation);

        other.gameObject.GetComponent<AudioSource>().Stop();
        other.gameObject.GetComponent<AudioSource>().PlayOneShot(portalSound);

        StartCoroutine(nameof(WarpCamera), other);
    }

    private IEnumerator WarpCamera(Collider other)
    {       
        other.gameObject.transform.root.transform.GetChild(0).GetChild(0).GetComponent<Animator>().Play("CameraWarpAnimation");

        yield return new WaitForSeconds(1);

        this.gameObject.GetComponent<BoxCollider>().enabled = true;
        tie.gameObject.GetComponent<BoxCollider>().enabled = true;

        other.gameObject.transform.root.transform.GetChild(0).GetChild(0).GetComponent<Animator>().Play("CameraIdle");
    }

}
