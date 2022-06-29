using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;

public class PlayerList : MonoBehaviourPunCallbacks
{
    [SerializeField] Transform container;
    [SerializeField] GameObject playerListCardPrefab;

    Dictionary<Player, PlayerListCard> playerListCards = new Dictionary<Player, PlayerListCard>();
    private void Start()
    {
        foreach(Player player in PhotonNetwork.PlayerList)
        {
            AddPlayerListCard(player);
        }
    }

    private void Update()
    {
        
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        AddPlayerListCard(newPlayer);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        RemovePlayerListCard(otherPlayer);
    }

    void AddPlayerListCard(Player player)
    {
        PlayerListCard card = Instantiate(playerListCardPrefab, container).GetComponent<PlayerListCard>();
        card.Initialize(player);
        playerListCards[player] = card;
    }

    void RemovePlayerListCard(Player player)
    {
        Destroy(playerListCards[player].gameObject);
        playerListCards.Remove(player);
    }
}
