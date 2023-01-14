using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayPlayerBeanAccessoriesMainMenu : MonoBehaviour
{
    private void OnEnable()
    {
        StartCoroutine(DisplayAccessories());
    }

    IEnumerator DisplayAccessories()
    {
        yield return new WaitUntil(predicate: () => FirebaseManager.Singleton.hasFixedDependencies);

        AccessoriesManager.Singleton.DisplayHats();
        AccessoriesManager.Singleton.DisplayEyewear();
        AccessoriesManager.Singleton.DisplayCapes();
    }
}
