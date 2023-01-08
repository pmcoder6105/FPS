using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LockerRoomAccessoriesButton : MonoBehaviour
{
    public int modifiedHat, modifiedEyewear, modifiedCape;
    public bool isHat, isEyeWear, isCape, removeAllHats, removeAllEyewear, removeAllCapes;

    public GameObject[] hatChecks;
    public GameObject[] eyewearChecks;
    public GameObject[] capeChecks;



    // Update is called once per frame
    void Start()
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
            GetComponent<Button>().onClick.AddListener(EquipCape);
        }
        if (removeAllHats)
        {
            GetComponent<Button>().onClick.AddListener(Btn_CancelHat);
        }
        if (removeAllEyewear)
        {
            GetComponent<Button>().onClick.AddListener(Btn_CancelEyewear);
        }
        if (removeAllCapes)
        {
            GetComponent<Button>().onClick.AddListener(Btn_CancelCapes);
        }

        
    }

    

    public void Btn_CancelHat()
    {
        AccessoriesManager.Singleton.removeHats = true;
        AccessoriesManager.Singleton.equipedHat = 0;
        StartCoroutine(FirebaseManager.Singleton.UpdateHats(0));

    }

    public void Btn_CancelEyewear()
    {
        AccessoriesManager.Singleton.removeEyewear = true;
        AccessoriesManager.Singleton.equipedEyewear = 0;
        StartCoroutine(FirebaseManager.Singleton.UpdateEyewear(0));

    }

    public void Btn_CancelCapes()
    {
        AccessoriesManager.Singleton.removeCapes = true;
        AccessoriesManager.Singleton.equipedCape = 0;
        StartCoroutine(FirebaseManager.Singleton.UpdateCapes(0));
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
