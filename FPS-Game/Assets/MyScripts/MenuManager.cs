using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;

    [SerializeField] Menu[] menus;

    FirebaseManager firebase;

    public GameObject titleMenu;
    public GameObject loadingMenu;

    private void Awake()
    {
        Instance = this;
        firebase = GameObject.Find("FirebaseManager").GetComponent<FirebaseManager>();
    }

    public void OpenMenu(string menuName)
    {
        for (int i = 0; i < menus.Length; i++)
        {
            if (menus[i].menuName == menuName)
            {
                menus[i].Open();
            }
            else if (menus[i].open)
            {
                CloseMenu(menus[i]);
            }
        }
    }

    private void Update()
    {
        if (titleMenu.activeInHierarchy == true)
            loadingMenu.SetActive(false);
    }

    public void OpenMenu(Menu menu)
    {
        for (int i = 0; i < menus.Length; i++)
        {
            if (menus[i].open)
            {
                CloseMenu(menus[i]);
            }
        }
        menu.Open();
    }

    public void CloseMenu(Menu menu)
    {
        menu.Close();
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void SignOut()
    {
        firebase.auth.SignOut();
        AccountUIManager.instance.menuCanvas.SetActive(false);
        AccountUIManager.instance.accountCanvas.SetActive(true);
        AccountUIManager.instance.mainCamera.transform.Find("PlayerViewer").gameObject.SetActive(false);
        //PhotonNetwork.Disconnect();
    }
}
