using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomizeManager : MonoBehaviour
{
    public GameObject customizeBeanModel;

    void Update()
    {

        if (this.gameObject.activeInHierarchy == true)
        {
            customizeBeanModel.SetActive(true);
        }
        if (this.gameObject.activeInHierarchy == false)
        {
            customizeBeanModel.SetActive(false);
        }
    }
}
