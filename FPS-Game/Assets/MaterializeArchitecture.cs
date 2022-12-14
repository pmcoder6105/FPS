using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterializeArchitecture : MonoBehaviour
{
    public Color colorOfArchitecture;
    public Shader materializeShader;
    Material architectureMatGlobal;

    public Texture proBuilder;

    // Update is called once per frame
    void Update()
    {
        if (Time.timeSinceLevelLoad <= Mathf.Epsilon)
        {
            Material architectureMat = new(materializeShader);
            architectureMatGlobal = architectureMat;
            architectureMatGlobal.SetTexture("ProBuilderTexture", proBuilder);
            architectureMatGlobal.SetFloat("EdgeWidth", 0.025f);

            this.gameObject.GetComponent<MeshRenderer>().material = architectureMatGlobal;
        }

        architectureMatGlobal.SetColor("Color", colorOfArchitecture);

        float elapsedTime = 0f;
        elapsedTime += Time.timeSinceLevelLoad;
        float percentComplete = elapsedTime / 2f;
        float lerpTime = Mathf.Lerp(0.5f, -0.5f, percentComplete);
        architectureMatGlobal.SetFloat("Dissolve", lerpTime);
        Debug.Log(lerpTime);


    }
}
