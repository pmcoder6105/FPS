using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LockerRoomAccessoriesButton : MonoBehaviour
{
    public int modifiedHat, modifiedEyewear, modifiedCape;
    public bool isHat, isEyeWear, isCape;

    public GameObject[] hatChecks;
    public GameObject[] eyewearChecks;
    public GameObject[] capeChecks;

    //public Color normalButtonColor;
    //public Color selectedButtonColor;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isHat)
        {
            GetComponent<Button>().onClick.AddListener(ModifyEquipedHat);
            GetComponent<Button>().onClick.AddListener(EquipHat);
        }
        if (isEyeWear)
        {
            GetComponent<Button>().onClick.AddListener(ModifyEquipedEyewear);
            GetComponent<Button>().onClick.AddListener(EquipEyewear);

        }        
        if (isCape)
        {
            GetComponent<Button>().onClick.AddListener(ModifyEquipedCape);
        }
    }

    public void ModifyEquipedHat()
    {
        AccessoriesManager.Singleton.equipedHat = modifiedHat;
    }    
    public void ModifyEquipedEyewear()
    {
        AccessoriesManager.Singleton.equipedEyewear = modifiedEyewear;

    }
    public void ModifyEquipedCape()
    {
        AccessoriesManager.Singleton.equipedCape = modifiedCape;
    }

    public void EquipHat()
    {
        foreach (GameObject item in hatChecks)
        {
            item.SetActive(false);
            //item.GetComponentInParent<Image>().color = normalButtonColor;
        }
        transform.GetChild(2).gameObject.SetActive(true);
        //GetComponent<Image>().color = selectedButtonColor;
    }
    public void EquipEyewear()
    {
        foreach (GameObject item in eyewearChecks)
        {
            item.SetActive(false);
            //item.GetComponentInParent<Image>().color = normalButtonColor;
        }
        transform.GetChild(2).gameObject.SetActive(true);
        //GetComponent<Image>().color = selectedButtonColor;
    }
    public void EquipCape()
    {
        foreach (GameObject item in capeChecks)
        {
            item.SetActive(false);
            //item.GetComponentInParent<Image>().color = normalButtonColor;
        }
        transform.GetChild(2).gameObject.SetActive(true);
        //GetComponent<Image>().color = selectedButtonColor;
    }
}
