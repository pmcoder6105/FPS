using UnityEngine;
using UnityEngine.UI;

public class Reticle : MonoBehaviour
{

    private RectTransform reticle; // The RecTransform of reticle UI element.

    public Rigidbody playerRigidbody;

    public float restingSize, maxSize, speed;
    private float currentSize;

    private void Start()
    {
        reticle = GetComponent<RectTransform>();
    }

    private void Update()
    {
        if (IsMoving)
        {
            currentSize = Mathf.Lerp(currentSize, maxSize, Time.deltaTime * speed);
        }
        else
        {
            currentSize = Mathf.Lerp(currentSize, restingSize, Time.deltaTime * speed);
        }
        reticle.sizeDelta = new Vector2(currentSize, currentSize);
    }

    // Bool to check if player is currently moving.
    bool IsMoving
    {
        get
        {
            if (Input.GetMouseButton(1))
            {
                return false;
            }
            if (
                Input.GetAxis("Horizontal") != 0 ||
                Input.GetAxis("Vertical") != 0 ||
                Input.GetMouseButton(0)
                    )
                return true;
            else
                return false;
        }

    }

}