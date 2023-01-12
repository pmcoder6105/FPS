using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayPlayerBeanAccessoriesMainMenu : MonoBehaviour
{
    private void OnEnable()
    {
        AccessoriesManager.Singleton.DisplayHats();
        AccessoriesManager.Singleton.DisplayEyewear();
        AccessoriesManager.Singleton.DisplayCapes();
    }
}
