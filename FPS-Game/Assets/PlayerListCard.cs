using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Realtime;

public class PlayerListCard : MonoBehaviour
{
    public TMP_Text usernameText;

    public void Initialize(Player player)
    {
        usernameText.text = player.NickName;
    }
}
