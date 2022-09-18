using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomizeManager : MonoBehaviour
{
    public bool isRedAndYellow = true;
    public bool isRedAndBlue = false;
    public bool isGreenAndBlue = false;
    public bool isBlackAndWhite = false;
    public int pillColorKey = 1;

    public GameObject customizeBeanModel;

    // Start is called before the first frame update
    void Start()
    {
        //PlayerPrefs.SetInt("PillColor", 1);
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(PlayerPrefs.GetInt("PillColor"));
        Debug.Log(pillColorKey);

        if (this.gameObject.activeInHierarchy == true)
        {
            customizeBeanModel.SetActive(true);
        }
        if (this.gameObject.activeInHierarchy == false)
        {
            customizeBeanModel.SetActive(false);
        }
    }

    public void ChangeToRedAndYellow()
    {
        isRedAndYellow = true;
        isRedAndBlue = false;
        isGreenAndBlue = false;
        isBlackAndWhite = false;
        pillColorKey = 1;
        PlayerPrefs.SetInt("PillColor", pillColorKey);
    }
    public void ChangeToRedAndBlue()
    {
        isRedAndYellow = false;
        isRedAndBlue = true;
        isGreenAndBlue = false;
        isBlackAndWhite = false;
        pillColorKey = 2;
        PlayerPrefs.SetInt("PillColor", pillColorKey);
    }
    public void ChangeToGreenAndBlue()
    {
        isRedAndYellow = false;
        isRedAndBlue = false;
        isGreenAndBlue = true;
        isBlackAndWhite = false;
        pillColorKey = 3;
        PlayerPrefs.SetInt("PillColor", pillColorKey);
    }
    public void ChangeToBlackAndWhite()
    {
        isRedAndYellow = false;
        isRedAndBlue = false;
        isGreenAndBlue = false;
        isBlackAndWhite = true;
        pillColorKey = 4;
        PlayerPrefs.SetInt("PillColor", pillColorKey);
    }
    public void ChangeToPurpleAndBlack()
    {
        pillColorKey = 5;
        PlayerPrefs.SetInt("PillColor", pillColorKey);
    }
    public void ChangeToBlueAndPurple()
    {
        pillColorKey = 6;
        PlayerPrefs.SetInt("PillColor", pillColorKey);
    }
}
