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

    FirebaseManager firebaseManager;

    private void Start()
    {
        firebaseManager = GameObject.Find("FirebaseManager").GetComponent<FirebaseManager>();
    }

    // Update is called once per frame
    void SavePlayerColor()
    {
        if (!PV.IsMine)
            return;

        matHealthy.SetColor("_MaterialColor", fcp.color);
        matNormal.SetColor("_MaterialColor", fcp.color);
        matHurt.SetColor("_MaterialColor", fcp.color);

        blockColor.color = fcp.color;

        //PlayerPrefs.SetString("BeanPlayerColor", ColorUtility.ToHtmlStringRGB(matHealthy.GetColor("_MaterialColor")));
        //fcp.TypeHex(PlayerPrefs.GetString("BeanPlayerColor"));

        firebaseManager.UpdatePlayerColor(ColorUtility.ToHtmlStringRGB(matHealthy.GetColor("_MaterialColor")));

        //LOAD DATA HERE
        //fcp.TypeHex()
    }

    private void OnEnable()
    {
        firebaseManager.LoadPlayerColorData(this.gameObject, fcp, matHealthy);
    }
}
