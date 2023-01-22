using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ParkourLeave : MonoBehaviour
{
    [SerializeField] GameObject leaveConfirm;
    [SerializeField] Button leaveButton;
    [SerializeField] GameObject player;
    [SerializeField] GameObject[] starters;

    // Start is called before the first frame update
    void Start()
    {
        leaveButton.onClick.AddListener(ProcessLeaveConfirmation);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("ESCAPE NOWWWWW");
            leaveConfirm.GetComponent<Animator>().Play("LeaveConfirmationTransition");
            leaveConfirm.GetComponent<CanvasGroup>().alpha = 1;
            Cursor.lockState = CursorLockMode.None;
        }
    }

    void ProcessLeaveConfirmation()
    {
        foreach (GameObject starter in starters)
        {
            starter.SetActive(true);
        }
        player.SetActive(false);
    }

    public void GoBackToLoadingScene()
    {
        SceneManager.LoadScene("LoadingScene");

    }

    public void CursorDisable()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
