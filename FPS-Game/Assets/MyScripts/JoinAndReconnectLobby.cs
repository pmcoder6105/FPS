using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class JoinAndReconnectLobby : MonoBehaviourPunCallbacks
{    
    //[SerializeField] GameObject reconnectBtn;
    [SerializeField] GameObject playerViewerParent;

    private static JoinAndReconnectLobby _singleton;

    private void Awake()
    {
        Singleton = this;
        DontDestroyOnLoad(this.gameObject);
    }

    private void Update()
    {
        if (Time.timeSinceLevelLoad <= Mathf.Epsilon)
        {
            if (SceneManager.GetActiveScene().name == "Menu")
            {
                OpenTitle();
            }
        }
        if (!AccountUIManager.instance.loadingMenu.activeSelf)
        {
            StopCoroutine("Reconnect");
        }
    }

    public static JoinAndReconnectLobby Singleton
    {
        get => _singleton;
        private set
        {
            if (_singleton == null)
            {
                _singleton = value;
            }
            else if (_singleton != value)
            {
                Debug.Log($"{nameof(AccessoriesManager)} instance already exists, destroying object!");
                Destroy(value.gameObject);
            }
            Debug.Log("Singleton called");
        }
    }

    public void OpenTitle()
    {
        StartCoroutine("Reconnect");        
    }

    private IEnumerator Reconnect()
    {
        Debug.Log("reconnect 1");
        yield return new WaitUntil(predicate: () => FirebaseManager.Singleton.hasFixedDependencies);
        yield return new WaitForSeconds(3f);

        Debug.Log("reconnect 3");
        AccountUIManager.instance.titleMenu.SetActive(true);
        AccountUIManager.instance.loadingMenu.SetActive(false);
        AccountUIManager.instance.playerViewer.SetActive(true);
    }

    public void Btn_Reconnect()
    {
        MenuManager.Instance.OpenMenu("title");
        AccountUIManager.instance.loadingMenu.SetActive(false);
        playerViewerParent.SetActive(true);
    }
}
