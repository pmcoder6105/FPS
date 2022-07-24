using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillBoard : MonoBehaviour
{
    GameObject cam;

    private void Update()
    {

        if (cam == null)
            cam = GameObject.FindGameObjectWithTag("MainCam");

        if (cam == null)
            return;

        transform.LookAt(cam.transform);
        transform.Rotate(Vector3.up * 180);
    }
}
