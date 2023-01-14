using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class KillFeed : MonoBehaviourPunCallbacks
{
    public GameObject killFeedTextPrefab; // Assign the prefab in the inspector
    public Transform killFeedParent; // Assign the parent object that holds all kill feed texts
    public ScrollRect scrollView; // assign the ScrollView in the inspector
    private float scrollViewHeight;

    // Callback function that is triggered when a player is killed
    [PunRPC]
    public void OnPlayerKilled(string killer, string victim)
    {
        // Instantiate the kill feed text prefab
        GameObject killFeedTextGO = Instantiate(killFeedTextPrefab, killFeedParent);

        // Set the text to display the names of the killer and victim
        killFeedTextGO.GetComponent<Text>().text = killer + " killed " + victim;

        // Make the text object a networked object
        PhotonNetwork.AllocateViewID(killFeedTextGO.GetPhotonView());
        killFeedTextGO.GetPhotonView().TransferOwnership(PhotonNetwork.LocalPlayer);
        killFeedTextGO.GetPhotonView().RPC("destroyText", RpcTarget.AllBuffered, null);

        // update the scroll view position to show the new text
        float lastTextHeight = killFeedTextGO.GetComponent<RectTransform>().rect.height;
        photonView.RPC("updateScrollView", RpcTarget.OthersBuffered, lastTextHeight);
    }

    [PunRPC]
    public void updateScrollView(float lastTextHeight)
    {
        scrollViewHeight += lastTextHeight;
        scrollView.content.sizeDelta = new Vector2(0, scrollViewHeight);
        scrollView.content.anchoredPosition = new Vector2(0, -scrollViewHeight);
    }
    [PunRPC]
    public void destroyText(GameObject text)
    {
        Destroy(text, 5); //destroy the text after 5 sec
    }

}
