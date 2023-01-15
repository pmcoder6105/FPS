using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using UnityEngine.UI;

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

    List<Material> customizeBeanMaterialsList = new();

    //public Button saveButton;

    // Update is called once per frame
    

    private void Update()
    {
        int number = customizeBeanMaterialsList.Count;
        if (customizeBeanMaterialsList.Count <= 0)
        {
            Material materialItem = customizeBeanMaterialsList[0];
            materialItem.SetColor("_MaterialColor", fcp.color);
            this.gameObject.transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).GetComponent<MeshRenderer>().material = materialItem;
        } else
        {
            Material item = customizeBeanMaterialsList[number - 1];
            item.SetColor("_MaterialColor", fcp.color);
            this.gameObject.transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).GetComponent<MeshRenderer>().material = item;
        }

        
    }


    public void SavePlayerColor()
    {
        if (!PV.IsMine)
            return;

        matHealthy.SetColor("_MaterialColor", fcp.color);
        matNormal.SetColor("_MaterialColor", fcp.color);
        matHurt.SetColor("_MaterialColor", fcp.color);

        blockColor.color = fcp.color;

        StartCoroutine(firebaseManager.UpdatePlayerColor(ColorUtility.ToHtmlStringRGB(this.gameObject.transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).GetComponent<MeshRenderer>().material.GetColor("_MaterialColor"))));
    }

    private void OnEnable()
    {
        if (!PV.IsMine)
            return;

        firebaseManager = GameObject.Find("FirebaseManager").GetComponent<FirebaseManager>();
        StartCoroutine(firebaseManager.LoadPlayerColorDataCustomizeBeanModel(gameObject, fcp, matHealthy));

        Material newMat = new(glowShader);
        customizeBeanMaterialsList.Add(newMat);
    }
}
