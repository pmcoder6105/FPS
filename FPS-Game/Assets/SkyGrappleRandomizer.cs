using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyGrappleRandomizer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        float y = Random.Range(19.1f, 30f);
        transform.position = new Vector3(transform.position.x, y, transform.position.z);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
