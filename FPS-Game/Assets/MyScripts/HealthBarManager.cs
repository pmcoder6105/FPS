using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class HealthBarManager : MonoBehaviour
{
    public PlayerController controller;

    public TMP_Text healthText;
    public GameObject glowFade;

    void Update()
    {
        Display();
    }

    void Display()
    {
        this.gameObject.GetComponent<Image>().fillAmount = controller.currentHealth / 100f;
        glowFade.GetComponent<Image>().fillAmount = controller.currentHealth / 100f;
        healthText.text = controller.currentHealth.ToString();
    }
}
