using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionUIElementManager : MonoBehaviour
{

    bool canFadeIn = true, canFadeOut = true;

    public string animationName;

    public GameObject animatorParent;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
    {
        animatorParent.GetComponent<Animator>().Play(animationName, 0 , 0f);
        Debug.Log("Should play transition animation now!");
        //if (canFadeIn)
        //    StartCoroutine(nameof(UIPopIn));
    }

    

    IEnumerator UIPopIn()
    {
        float elapsedTime = 0f;
        elapsedTime += Time.timeSinceLevelLoad;
        float percentComplete = elapsedTime / 0.07f;
        float scaleLerpTime = Mathf.Lerp(0.95f, 1f, percentComplete);
        float alphaLerpTime = Mathf.Lerp(0f, 1f, percentComplete);

        GetComponent<CanvasGroup>().alpha = alphaLerpTime;
        GetComponent<RectTransform>().localScale = new Vector3(alphaLerpTime, alphaLerpTime, 0);

        

        canFadeOut = false;

        yield return new WaitUntil(predicate: () => alphaLerpTime == 1f);
        canFadeOut = true;


    }

    IEnumerator UIPopOut()
    {
        float elapsedTime = 0f;
        elapsedTime += Time.timeSinceLevelLoad;
        float percentComplete = elapsedTime / 0.07f;
        float scaleLerpTime = Mathf.Lerp(1f, 0.95f, percentComplete);
        float alphaLerpTime = Mathf.Lerp(1f, 0f, percentComplete);

        GetComponent<CanvasGroup>().alpha = alphaLerpTime;
        GetComponent<RectTransform>().localScale = new Vector3(alphaLerpTime, alphaLerpTime, 0);

        canFadeIn = false;

        yield return new WaitUntil(predicate: () => alphaLerpTime == 1f);
        canFadeIn = true;


    }

    //private void OnDisable()
    //{
    //    if (canFadeOut)
    //        StartCoroutine(nameof(UIPopOut));
    //}
}
