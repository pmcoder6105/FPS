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
    void OnEnable()
    {
        if (isHat)
        {
            GetComponent<Button>().onClick.AddListener(ModifyEquipedHat);
        }
        else if (isEyeWear)
        {
            GetComponent<Button>().onClick.AddListener(ModifyEquipedEyewear);
        }        
        else if (isCape)
        {
            GetComponent<Button>().onClick.AddListener(ModifyEquipedCape);
        }
        if (removeAllHats)
        {
            GetComponent<Button>().onClick.AddListener(Btn_CancelHat);
        }
        else if (removeAllEyewear)
        {
            GetComponent<Button>().onClick.AddListener(Btn_CancelEyewear);
        }
        else if (removeAllCapes)
        {
            GetComponent<Button>().onClick.AddListener(Btn_CancelCapes);
        }
    }

    

    public void Btn_CancelHat()
    {
        AccessoriesManager.Singleton.removeHats = true;
        AccessoriesManager.Singleton.equipedHat = 0;
        StartCoroutine(FirebaseManager.Singleton.UpdateRemoveHats(true));

    }
    public void Btn_CancelEyewear()
    {
        AccessoriesManager.Singleton.removeEyewear = true;
        AccessoriesManager.Singleton.equipedEyewear = 0;
        StartCoroutine(FirebaseManager.Singleton.UpdateRemoveEyewear(true));

    }
    public void Btn_CancelCapes()
    {
        AccessoriesManager.Singleton.removeCapes = true;
        AccessoriesManager.Singleton.equipedCape = 0;
        StartCoroutine(FirebaseManager.Singleton.UpdateRemoveCapes(true));
    }
    public void ModifyEquipedHat()
    {
        AccessoriesManager.Singleton.removeHats = false;

        AccessoriesManager.Singleton.equipedHat = modifiedHat;
        StartCoroutine(FirebaseManager.Singleton.UpdateRemoveHats(false));
        StartCoroutine(Equip(AccountUIManager.instance.hatChecks));

    }
    public void ModifyEquipedEyewear()
    {
        AccessoriesManager.Singleton.removeEyewear = false;

        AccessoriesManager.Singleton.equipedEyewear = modifiedEyewear;
        
        StartCoroutine(FirebaseManager.Singleton.UpdateRemoveEyewear(false));
        StartCoroutine(Equip(AccountUIManager.instance.eyeChecks));

    }
    public void ModifyEquipedCape()
    {
        AccessoriesManager.Singleton.removeCapes = false;

        AccessoriesManager.Singleton.equipedCape = modifiedCape;
        StartCoroutine(FirebaseManager.Singleton.UpdateRemoveCapes(false));
        StartCoroutine(Equip(AccountUIManager.instance.capeChecks));
    }

    public IEnumerator Equip(GameObject[] check)
    {
        int point = 0;
        bool done = false;

        foreach (GameObject item in check)
        {
            point++;
            item.SetActive(false);
            if (point == check.Length)
            {
                done = true;
            }
        }
        yield return new WaitUntil(predicate: () => done == true);
        transform.GetChild(2).gameObject.SetActive(true);
    }    
}
