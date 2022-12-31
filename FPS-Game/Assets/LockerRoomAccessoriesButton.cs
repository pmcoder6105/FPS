using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LockerRoomAccessoriesButton : MonoBehaviour
{
    public int modifiedHat, modifiedEyewear, modifiedExternal, modifiedCape;
    public bool isHat, isEyeWear, isExternal, isCape;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isHat)
            GetComponent<Button>().onClick.AddListener(ModifyEquipedHat);

        if (isEyeWear)
            GetComponent<Button>().onClick.AddListener(ModifyEquipedEyewear);

        if (isExternal)
            GetComponent<Button>().onClick.AddListener(ModifyEquipedExternal);

        if (isCape)
            GetComponent<Button>().onClick.AddListener(ModifyEquipedCape);
    }

    public void ModifyEquipedHat()
    {
        AccessoriesManager.Singleton.equipedHat = modifiedHat;
    }    
    public void ModifyEquipedEyewear()
    {
        AccessoriesManager.Singleton.equipedEyewear = modifiedEyewear;

    }
    public void ModifyEquipedExternal()
    {

    }
    public void ModifyEquipedCape()
    {

    }
}
