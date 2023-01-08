using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayPlayerBeanAccessoriesMainMenu : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        AccessoriesManager.Singleton.DisplayAccessories();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
