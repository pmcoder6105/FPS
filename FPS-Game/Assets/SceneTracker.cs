using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneTracker : MonoBehaviour
{

    public static bool hasLoggedIn = false; 

    public void PreventReLogIn()
    {
        hasLoggedIn = true;
    }

    public void SignOut()
    {
        hasLoggedIn = false;
    }
}
