using Photon.Realtime;
using Photon.Pun;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using TMPro;
using UnityEngine.SceneManagement;

public class ScoreBoard : MonoBehaviourPunCallbacks
{
    [SerializeField] Transform container;
    [SerializeField] GameObject scoreBoardItemPrefab;
    [SerializeField] CanvasGroup canvasGroup;

    [SerializeField] CanvasGroup scoreBoardTitle;
    [SerializeField] TMP_Text holdTabForScoreBoardText;

    public CanvasGroup areYouSureYouWantToLeaveConfirmation;

    public bool isOpen = false;

    Dictionary<Player, ScoreBoardItem> scoreBoardItems = new Dictionary<Player, ScoreBoardItem>();

    private void Start()
    {
        foreach(Player player in PhotonNetwork.PlayerList)
        {
            AddScoreBoardItem(player);
        }
        scoreBoardTitle.alpha = 0;
        areYouSureYouWantToLeaveConfirmation.alpha = 0;
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        AddScoreBoardItem(newPlayer);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        RemoveScoreBoardItem(otherPlayer);
    }

    void AddScoreBoardItem(Player player)
    {
        ScoreBoardItem item = Instantiate(scoreBoardItemPrefab, container).GetComponent<ScoreBoardItem>();
        item.Initialize(player);
        scoreBoardItems[player] = item;
    }

    void RemoveScoreBoardItem(Player player)
    {
        Destroy(scoreBoardItems[player].gameObject);
        scoreBoardItems.Remove(player);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            canvasGroup.alpha = 1;
            isOpen = true;
            scoreBoardTitle.alpha = 1;
            holdTabForScoreBoardText.color = new Color(holdTabForScoreBoardText.color.r, holdTabForScoreBoardText.color.g, holdTabForScoreBoardText.color.b, 0);
            Cursor.lockState = CursorLockMode.None;
        }
        if (Input.GetKeyUp(KeyCode.Tab))
        {
            canvasGroup.alpha = 0;
            isOpen = false;
            scoreBoardTitle.alpha = 0;
            holdTabForScoreBoardText.color = new Color(holdTabForScoreBoardText.color.r, holdTabForScoreBoardText.color.g, holdTabForScoreBoardText.color.b, 1);
        }
        
        
    }

    public void OpenLeaveConfirmation()
    {

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            areYouSureYouWantToLeaveConfirmation.alpha = 1;
            Cursor.lockState = CursorLockMode.None;
            isOpen = true;
        }        
    }

    
}
