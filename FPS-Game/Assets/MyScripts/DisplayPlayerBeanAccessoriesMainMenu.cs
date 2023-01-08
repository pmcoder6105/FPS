using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayPlayerBeanAccessoriesMainMenu : MonoBehaviour
{
    private void OnEnable()
    {
        AccessoriesManager.Singleton.DisplayAccessories();
    }
}
