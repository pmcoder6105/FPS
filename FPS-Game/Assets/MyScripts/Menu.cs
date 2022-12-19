using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{
    public string menuName;
    public bool open;

    FirebaseManager firebase;
    public GameObject mainMenuBeanModel;

    public FlexibleColorPicker fcp;

    public Material healthyMat;

    [SerializeField] bool fadeIn = true;

    private void Awake()
    {
        //firebase = GameObject.Find("FirebaseManager").GetComponent<FirebaseManager>();
        //GameObject.FindObjectsOfType(FirebaseManager);
        firebase = GameObject.Find("FirebaseManager").GetComponent<FirebaseManager>();
    }

    private void Update()
    {
        if (fadeIn)
        {
            //StartCoroutine(nameof(FadeMenuIn));
        }
        
    }

    public void Open()
    {
        open = true;
        gameObject.SetActive(true);

        if (menuName == "title")
        {
            Debug.Log(firebase);
            StartCoroutine(firebase.LoadPlayerColorDataMainMenuBeanModel(mainMenuBeanModel, fcp, healthyMat));
        }
    }

    public void Close()
    {
        open = false;
        gameObject.SetActive(false);
    }

    

    IEnumerator FadeMenuIn()
    {
        float elapsedTime = 0f;
        elapsedTime += Time.timeSinceLevelLoad;
        float percentComplete = elapsedTime / 2;
        float lerpTime = Mathf.Lerp(0, 1, percentComplete);
        Debug.Log(lerpTime);

        GetComponent<CanvasGroup>().alpha = lerpTime;

        yield return new WaitUntil(predicate: () => lerpTime == 0);

        fadeIn = false;
    }

    private void OnDisable()
    {
        GetComponent<CanvasGroup>().alpha = 0;
        fadeIn = true;
    }
}
