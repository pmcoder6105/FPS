using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AccessoriesManager : MonoBehaviour
{
    public int equipedHat, equipedEyewear, equipedCape;

    public bool removeHats, removeEyewear, removeCapes;

    private static AccessoriesManager _singleton;

    //public GameObject[] hats, eyewear, capes;

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

    public void DisplayAccessories()
    {
        for (int i = 0; i < AccountUIManager.instance.hats.Length; i++)
        {
            if (i != equipedHat)
            {
                AccountUIManager.instance.hats[i].SetActive(false);
            }
            else
            {
                if (i == 0)
                    return;
                AccountUIManager.instance.hats[i].SetActive(true);
            }
        }

        for (int i = 0; i < AccountUIManager.instance.eyewear.Length; i++)
        {
            if (i != equipedHat)
            {
                AccountUIManager.instance.eyewear[i].SetActive(false);
            }
            else
            {
                if (i == 0)
                    return;
                AccountUIManager.instance.eyewear[i].SetActive(true);
            }
        }

        for (int i = 0; i < AccountUIManager.instance.capes.Length; i++)
        {
            if (i != equipedHat)
            {
                AccountUIManager.instance.capes[i].SetActive(false);
            }
            else
            {
                if (i == 0)
                    return;
                AccountUIManager.instance.capes[i].SetActive(true);
            }
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
        StartCoroutine(nameof(FirebaseManager.Singleton.UpdateHats));
    }
    public void SaveEyewear()
    {
        StartCoroutine(nameof(FirebaseManager.Singleton.UpdateHats));
    }
    public void SaveCapes()
    {
        StartCoroutine(nameof(FirebaseManager.Singleton.UpdateHats));
    }
}
