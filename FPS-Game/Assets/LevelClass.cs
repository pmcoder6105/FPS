using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelClass : MonoBehaviour
{

    public int level;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        level = LevelUpManager.Singleton.currentLevel;
    }
}
