using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccessoriesManager : MonoBehaviour
{
    public int equipedHat;
    public int equipedEyewear;

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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //TODO: MAKE VARIABLES IN THE ACCOUNT MANAGER FOR THE ACCESSORIES BUTTONS
    //

}
