using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    [Header("References")]
    [SerializeField] WallRun wallRun;

    [SerializeField] private float sensX = 100f;
    [SerializeField] private float sensY = 100f;

    [SerializeField] Transform cam ;
    [SerializeField] Transform orientation;

    float verticalLookRotation;

    float mouseX;
    float mouseY;

    float multiplier = 0.01f;

    float xRotation;
    float yRotation;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void FixedUpdate()
    {
        transform.Rotate(Input.GetAxisRaw("Mouse X") * 3 * Vector3.up);

        verticalLookRotation += Input.GetAxisRaw("Mouse Y") * 3;
        verticalLookRotation = Mathf.Clamp(verticalLookRotation, -90f, 90f);

        cam.transform.localEulerAngles = Vector3.left * verticalLookRotation;
        orientation.transform.localEulerAngles = Vector3.left * verticalLookRotation;
    }
}
