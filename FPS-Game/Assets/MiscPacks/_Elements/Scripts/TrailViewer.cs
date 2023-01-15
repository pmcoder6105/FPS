using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class TrailViewer : MonoBehaviour
{
    public CinemachineDollyCart cart;
    public ShowcaseManager sm;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        sm.selectedEffect.transform.position = cart.transform.position;
    }
}
