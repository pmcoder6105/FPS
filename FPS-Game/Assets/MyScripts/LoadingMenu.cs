using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class LoadingMenu : MonoBehaviour
{

    //public AnimationClip clip1, clip2, clip3;

    public bool isDone;

    public TMP_Text tipText;

    [SerializeField] string[] tips;

    [SerializeField] int amountOfTips;
    // Start is called before the first frame update

    [SerializeField] GameObject chooser;

    [SerializeField] Button[] chooserButtons;

    [SerializeField] GameObject loading;

    [SerializeField] GameObject canvas;

    bool stopSFX = false;

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
            chooser.SetActive(true);
            StopCoroutine("GenerateTip");
            canvas.GetComponent<AudioSource>().enabled = true;
            //chooserButtons[0].onClick.AddListener(FreeForAllMenu);
            //chooserButtons[1].onClick.AddListener(FreeForAllMenu);
            //chooserButtons[2].onClick.AddListener(FreeForAllMenu);
        }
    }

    public void FreeForAllMenu()
    {
        loading.SetActive(true);
        
        SceneManager.LoadScene("Menu");
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
