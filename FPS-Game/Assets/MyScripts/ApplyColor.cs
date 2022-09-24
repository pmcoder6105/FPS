using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ApplyColor : MonoBehaviour
{
    public PhotonView PV;

    public FlexibleColorPicker fcp;
    public Material matHealthy;
    public Material matNormal;
    public Material matHurt;

    public Material blockColor;

    public GameObject customizeMenuBeanModelGameObject;

    public Shader glowShader;

    // Update is called once per frame
    void Update()
    {
        if (!PV.IsMine)
            return;

        matHealthy.SetColor("_MaterialColor", fcp.color);
        matNormal.SetColor("_MaterialColor", fcp.color);
        matHurt.SetColor("_MaterialColor", fcp.color);

        blockColor.color = fcp.color;

        PlayerPrefs.SetString("BeanPlayerColor", ColorUtility.ToHtmlStringRGB(matHealthy.GetColor("_MaterialColor")));
        fcp.TypeHex(PlayerPrefs.GetString("HealthyColor"));
    }
}
