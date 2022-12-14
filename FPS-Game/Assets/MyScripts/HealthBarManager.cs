using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class HealthBarManager : MonoBehaviour
{

    //public Shader healthBar;

    //public Texture2D squareTex;

    public PlayerController controller;

    public TMP_Text healthText;

    //public Material globalHealthyMat;

    // Start is called before the first frame update
    
    
    
    
    //void Awake()
    //{
    //    Material healthBarMat = new(healthBar);

    //    //globalHealthyMat = healthBarMat;
    //    controller = transform.root.gameObject.GetComponent<PlayerController>();
    //    this.gameObject.GetComponent<Image>().material = healthBarMat;
    //}

    // Update is called once per frame
    void Update()
    {
        Debug.Log("current health: " + controller.currentHealth);

        Debug.Log("current mat: " + this.GetComponent<Image>().material);
        //this.GetComponent<Image>().material.SetFloat("_RemovedSegments", controller.currentHealth / 10);
        this.gameObject.GetComponent<Image>().fillAmount = controller.currentHealth / 100f;

        healthText.text = controller.currentHealth.ToString();

    }
}
