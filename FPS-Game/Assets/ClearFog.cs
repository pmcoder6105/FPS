using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ClearFog : MonoBehaviour
{

    public void Clear(PhotonView pv)
    {
        if (pv.IsMine)
        {
            GetComponent<Animator>().Play("ClearFogAnim");
        }
    }

    private void Update()
    {
        if (!transform.root.gameObject.GetComponent<PhotonView>().IsMine)
            return;

        RenderSettings.fogDensity = this.transform.position.x;
    }
}
