using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionUIElementManager : MonoBehaviour
{
    public string animationName;

    public GameObject animatorParent;

    private void OnEnable()
    {
        animatorParent.GetComponent<Animator>().Play(animationName, 0 , 0f);
        Debug.Log("Should play transition animation now!");
    }
}
