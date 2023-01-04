using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SocialHyperLinks : MonoBehaviour
{
    public void OpenURL(string URL)
    {
        Application.OpenURL(URL);
    }
}
