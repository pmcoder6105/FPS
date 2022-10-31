using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Photon.Pun;

public class PlayerNameManager : MonoBehaviour
{
    [SerializeField] TMP_InputField usernameInput;

    FirebaseManager firebase;

    private void Awake()
    {
        firebase = GameObject.Find("FirebaseManager").GetComponent<FirebaseManager>();
    }

    private void Update()
    {
        if (PlayerPrefs.HasKey("username"))
        {
            //usernameInput.text = PlayerPrefs.GetString("username");
            //PhotonNetwork.NickName = PlayerPrefs.GetString("username");

            //GET DATA HERE
            //firebase.LoadUsernameData(usernameInput);
            //StartCoroutine(firebase.LoadUsernameData(usernameInput));
        }
        else // SAVE DATA HERE
        {
            //usernameInput.text = "Guest " + Random.Range(0, 1000).ToString("000");
            //OnUserNameInputValueChanged();
        }
    }

    public void OnUserNameInputValueChanged()
    {
        //PhotonNetwork.NickName = usernameInput.text;
        //PlayerPrefs.SetString("username", usernameInput.text);
        //firebase.SaveUsernameData(usernameInput.text);
    }
}
