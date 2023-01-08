using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccessoriesManager : MonoBehaviour
{
    public int equipedHat, equipedEyewear, equipedCape;

    public bool removeHats, removeEyewear, removeCapes;

    private static AccessoriesManager _singleton;

    public GameObject[] hats, eyewear, capes;

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
        for (int i = 0; i < hats.Length; i++)
        {
            if (i != equipedHat)
            {
                hats[i].SetActive(false);
            }
            else
            {
                if (i == 0)
                    return;
                hats[i].SetActive(true);
            }
        }

        for (int i = 0; i < eyewear.Length; i++)
        {
            if (i != equipedHat)
            {
                eyewear[i].SetActive(false);
            }
            else
            {
                if (i == 0)
                    return;
                eyewear[i].SetActive(true);
            }
        }

        for (int i = 0; i < capes.Length; i++)
        {
            if (i != equipedHat)
            {
                capes[i].SetActive(false);
            }
            else
            {
                if (i == 0)
                    return;
                capes[i].SetActive(true);
            }
        }
    }
}
