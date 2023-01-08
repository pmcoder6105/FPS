using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuVisualReward : MonoBehaviour
{
    public string rewardName;
    public bool open;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Open()
    {
        open = true;
        gameObject.SetActive(true);
    }

    public void Close()
    {
        open = false;
        gameObject.SetActive(false);
    }
}
