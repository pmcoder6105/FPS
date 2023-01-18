using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ClearFog : MonoBehaviour
{
    public bool shouldClear = false;

    public void Clear(PhotonView pv)
    {
        if (pv.IsMine)
        {
            GetComponent<Animator>().Play("ClearFogAnim");
            shouldClear = true;
        }
    }

    private void Update()
    {
        if (!transform.root.gameObject.GetComponent<PhotonView>().IsMine)
            return;

        if (!shouldClear)
            return;

        RenderSettings.fogDensity = transform.localPosition.x;
    }
}
