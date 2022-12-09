using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamageNumber : MonoBehaviour
{
    [SerializeField] private float destroyTime;
    [SerializeField] private Vector3 offset;
    [SerializeField] Color color;

    TextMeshPro textMeshPro;

    // Start is called before the first frame update
    void Start()
    {
        textMeshPro = GetComponent<TextMeshPro>();
        transform.localPosition += offset;
        Destroy(gameObject, destroyTime);
    }

    public void Initialize(float damage)
    {
        textMeshPro.text = damage.ToString();
    }
}
