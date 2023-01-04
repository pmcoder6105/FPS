using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XPAndLevelVisualMainMenu : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(LevelUpManager.Singleton.LoadXPAndKills());
    }
}
