using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplyColor : MonoBehaviour
{
    public FlexibleColorPicker fcp;
    public Material matHealthy;
    public Material matNormal;
    public Material matHurt;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //mat.color = fcp.color;
        matHealthy.SetColor("_MaterialColor", fcp.color);
        matNormal.SetColor("_MaterialColor", fcp.color);
        matHurt.SetColor("_MaterialColor", fcp.color);



        PlayerPrefs.SetString("HealthyColor", ColorUtility.ToHtmlStringRGB(matHealthy.GetColor("_MaterialColor")));

        fcp.TypeHex(PlayerPrefs.GetString("HealthyColor"));

        //fcp.color = ColorUtility.TryParseHtmlString(PlayerPrefs.GetString("HealthyColor"), out matHealthy.GetColor("_MaterialColor"));

        ////fcp.color. = PlayerPrefs.GetString("HealthyColor");
    }
}
