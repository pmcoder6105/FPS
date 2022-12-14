using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBarManager : MonoBehaviour
{

    Shader healthBar;

    Texture2D squareTex;

    PlayerController controller;

    public Material globalHealthyMat;

    private void Awake()
    {
        controller = GetComponent<PlayerController>();
    }

    // Start is called before the first frame update
    void Start()
    {
        Material healthBarMat = new(healthBar);

        globalHealthyMat = healthBarMat;

        this.gameObject.GetComponent<SpriteRenderer>().material = healthBarMat;
        healthBarMat.SetTexture("MainTex", squareTex);
        healthBarMat.SetFloat("Radius", 0.38f);
        healthBarMat.SetFloat("LineWidth", 0.05f);
        healthBarMat.SetFloat("Rotation", 16.93f);
        healthBarMat.SetFloat("RemovedSegments", 1);
        healthBarMat.SetFloat("SegmentSpacing", 0.03f);
        healthBarMat.SetFloat("SegmentCount", 11);
    }

    // Update is called once per frame
    void Update()
    {
        this.GetComponent<SpriteRenderer>().material.SetFloat("RemovedSegments", controller.currentHealth);
    }
}
