using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;

public class LevelUpManager : MonoBehaviour
{
    private static LevelUpManager _singleton;

    public int currentLevel;

    public int currentExperience;
    int experienceToNextLevel;

    private ExitGames.Client.Photon.Hashtable _props = new ExitGames.Client.Photon.Hashtable();

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
            StartCoroutine(LoadXPAndKills());
        }
    }

    public IEnumerator LoadXPAndKills()
    {
        yield return new WaitUntil(predicate: () => FirebaseManager.Singleton.hasFixedDependencies == true);

        StartCoroutine(FirebaseManager.Singleton.LoadExperience());
        StartCoroutine(FirebaseManager.Singleton.LoadKills());
    }

    public void CheckLevelUp(int _currentExperience, GameObject _empty, PhotonView pv)
    {
        if (pv.IsMine == false)
            return;

        Debug.Log(_currentExperience);

        if (_currentExperience >= 1) //JUST FOR PLAYTESTING! MAKE SURE TO REVERT BACK TO 20 AFTER PLAYTESTING
        {
            LevelUp();
            currentExperience = 0;
            StartCoroutine(FirebaseManager.Singleton.UpdateExperience(0));

            Debug.Log("should level up now!");

            _empty.SetActive(true);
            _empty.GetComponent<Animator>().Play("LevelUpAnimation");
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

        FirebaseManager.Singleton._customProps["playerLevel"] = currentLevel;
        PhotonNetwork.LocalPlayer.CustomProperties = FirebaseManager.Singleton._customProps;

        Debug.Log("Manager's properties " + PhotonNetwork.LocalPlayer.CustomProperties["playerLevel"]);
    }

    public void AddExperiencePoints()
    {
        currentExperience++;
        StartCoroutine(FirebaseManager.Singleton.UpdateExperience(currentExperience));
    }
}
