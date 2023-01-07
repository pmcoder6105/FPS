using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
}
