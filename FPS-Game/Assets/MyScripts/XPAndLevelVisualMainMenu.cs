using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XPAndLevelVisualMainMenu : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(LevelUpManager.Singleton.LoadXPAndKills());
    }
}
