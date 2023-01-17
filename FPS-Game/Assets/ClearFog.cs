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
            RenderSettings.fogDensity = this.transform.position.x;

        }
    }
}
