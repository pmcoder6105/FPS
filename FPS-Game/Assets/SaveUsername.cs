using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SaveUsername : MonoBehaviour
{
    [SerializeField] TMP_InputField userString;


    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(Save);
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void Save()
    {
        FirebaseManager.Singleton.SaveUsernameData();
    }
}
