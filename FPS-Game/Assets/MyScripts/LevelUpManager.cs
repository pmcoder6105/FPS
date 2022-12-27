using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using System.Linq;

public class LevelUpManager : MonoBehaviour
{
    private static LevelUpManager _singleton;

    public int currentLevel;

    public int currentExperience;
    int experienceToNextLevel;

    //FirebaseManager firebase;

    //PhotonView PV;

    private void Awake()
    {
        Singleton = this;
        DontDestroyOnLoad(this.gameObject);
        //firebase = GameObject.Find("FirebaseManager").GetComponent<FirebaseManager>();
        //PV = GetComponent<PhotonView>();
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
                Debug.Log($"{nameof(LevelUpManager)} instance already exists, destroying object!");
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

            StartCoroutine(LoadXPAndKills());
        }
    }

    public IEnumerator LoadXPAndKills()
    {
        yield return new WaitUntil(predicate: () => FirebaseManager.Singleton.hasFixedDependencies == true);

        StartCoroutine(FirebaseManager.Singleton.LoadExperience());
        StartCoroutine(FirebaseManager.Singleton.LoadKills());
    }

    public void CheckLevelUp(int _currentExperience, GameObject _empty)
    {
        Debug.Log(_currentExperience);

        if (_currentExperience >= 1) //JUST FOR PLAYTESTING! MAKE SURE TO REVERT BACK TO 20 AFTER PLAYTESTING
        {
            LevelUp();
            currentExperience = 0;
            StartCoroutine(FirebaseManager.Singleton.UpdateExperience(0));

            Debug.Log("should level up now!");

            _empty.SetActive(true);
            _empty.GetComponent<Animator>().Play("LevelUpAnimation", 0, 0f);
            StartCoroutine(nameof(DisableLevelUpAnimation), _empty);
        }

        
    }


    private IEnumerator DisableLevelUpAnimation(GameObject __empty)
    {
        yield return new WaitForSeconds(3f);
        __empty.SetActive(false);
    }

    public void LevelUp()
    {
        currentLevel++;
        StartCoroutine(FirebaseManager.Singleton.UpdateKills(currentLevel));

        if (PhotonNetwork.LocalPlayer.CustomProperties["playerLevel"] == null)
        {
            Hashtable hash = new();
            hash.Add("playerLevel", currentLevel);
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        }
        else PhotonNetwork.LocalPlayer.CustomProperties["playerLevel"] = currentLevel;
    }

    public void AddExperiencePoints()
    {
        currentExperience++;
        StartCoroutine(FirebaseManager.Singleton.UpdateExperience(currentExperience));
    }
}
