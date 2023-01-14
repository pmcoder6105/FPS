using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AccessoriesManager : MonoBehaviour
{
    public int equipedHat, equipedEyewear, equipedCape;

    public bool removeHats, removeEyewear, removeCapes;

    private static AccessoriesManager _singleton;

    private void Awake()
    {
        Singleton = this;
        DontDestroyOnLoad(this.gameObject);
    }


    public static AccessoriesManager Singleton
    {
        get => _singleton;
        private set
        {
            if (_singleton == null)
            {
                _singleton = value;
            }
            else if (_singleton != value)
            {
                Debug.Log($"{nameof(AccessoriesManager)} instance already exists, destroying object!");
                Destroy(value.gameObject);
            }
            Debug.Log("Singleton called");
        }
    }

    public void DisplayHats()
    {
        if (!removeHats)
        {
            for (int i = 0; i < AccountUIManager.instance.hats.Length; i++)
            {
                AccountUIManager.instance.hats[i].SetActive(false);
            }

            AccountUIManager.instance.hats[equipedHat].SetActive(true);

            Debug.Log("The activated hat " + AccountUIManager.instance.hats[equipedHat]);
            Debug.Log("The equiped hat var " + equipedHat);
        }
    }

    public void DisplayEyewear()
    {
        if (!removeEyewear)
        {
            for (int i = 0; i < AccountUIManager.instance.eyewear.Length; i++)
            {
                AccountUIManager.instance.eyewear[i].SetActive(false);
            }

            AccountUIManager.instance.eyewear[equipedEyewear].SetActive(true);

            Debug.Log("The activated eye " + AccountUIManager.instance.eyewear[equipedEyewear]);
            Debug.Log("The equiped eye var " + equipedEyewear);
        }
    }

    public void DisplayCapes()
    {
        if (!removeCapes)
        {
            for (int i = 0; i < AccountUIManager.instance.capes.Length; i++)
            {
                AccountUIManager.instance.capes[i].SetActive(false);
            }

            AccountUIManager.instance.capes[equipedCape].SetActive(true);

            Debug.Log("The activated cape " + AccountUIManager.instance.capes[equipedCape]);
            Debug.Log("The equiped cape var " + equipedCape);
        }
    }

    private void Start()
    {
        AccountUIManager.instance.saveButtons[0].onClick.AddListener(SaveHats);
        AccountUIManager.instance.saveButtons[1].onClick.AddListener(SaveEyewear);
        AccountUIManager.instance.saveButtons[2].onClick.AddListener(SaveCapes);
    }

    public void SaveHats()
    {
        StartCoroutine(FirebaseManager.Singleton.UpdateHats(equipedHat));
    }
    public void SaveEyewear()
    {
        StartCoroutine(FirebaseManager.Singleton.UpdateEyewear(equipedEyewear));
    }
    public void SaveCapes()
    {
        StartCoroutine(FirebaseManager.Singleton.UpdateCapes(equipedCape));
    }
}
