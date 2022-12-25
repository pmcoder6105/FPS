using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class LoadingMenu : MonoBehaviour
{

    //public AnimationClip clip1, clip2, clip3;

    public bool isDone;

    public TMP_Text tipText;

    [SerializeField] string[] tips;

    [SerializeField] int amountOfTips;
    // Start is called before the first frame update
    void Start()
    {
        int loadingClip = Random.Range(1, 4);

        GetComponent<Animator>().Play("LoadingSceneLogo" + loadingClip);

        StartCoroutine("GenerateTip");
    }

    // Update is called once per frame
    void Update()
    {
        if (isDone)
        {
            //Load Main Menu
            SceneManager.LoadScene("Menu");
            StopCoroutine("GenerateTip");
        }
    }

    private IEnumerator GenerateTip()
    {
        int tipInfo = Random.Range(0, amountOfTips);

        tipText.text = tips[tipInfo];

        yield return new WaitForSeconds(5);

        int tipInfo2 = Random.Range(0, amountOfTips);

        tipText.text = tips[tipInfo2];

        //StartCoroutine(nameof(GenerateTip));
    }
}
