using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelUpManager : MonoBehaviour
{
    private static LevelUpManager _singleton;

    int currentLevel;

    int currentExperience;
    int experienceToNextLevel;

    private void Awake()
    {
        Singleton = this;
        DontDestroyOnLoad(this.gameObject);
        
    }


    public static LevelUpManager Singleton
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
                Debug.Log($"{nameof(FirebaseManager)} instance already exists, destroying object!");
                Destroy(value.gameObject);
            }
            Debug.Log("Singleton called");
        }
    }

    private void Update()
    {
        if (Time.timeSinceLevelLoad <= Mathf.Epsilon)
        {
            //LoadLevel
            //LoadExperience
        }
    }

    public void CheckLevelUp(int _currentExperience)
    {
        if (_currentExperience >= 1) //JUST FOR PLAYTESTING! MAKE SURE TO REVERT BACK TO 20 AFTER PLAYTESTING
        {
            LevelUp();
            currentExperience = 0;
            StartCoroutine(FirebaseManager.Singleton.UpdateExperience(0));
        }
    }

    public void LevelUp()
    {
        currentLevel++;
        StartCoroutine(FirebaseManager.Singleton.UpdateKills(currentLevel));
    }

    public void AddExperiencePoints()
    {
        currentExperience++;
        StartCoroutine(FirebaseManager.Singleton.UpdateExperience(currentExperience));
    }
}
