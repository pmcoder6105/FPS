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

    FirebaseManager firebase;

    PhotonView PV;

    private void Awake()
    {
        Singleton = this;
        DontDestroyOnLoad(this.gameObject);
        firebase = GameObject.Find("FirebaseManager").GetComponent<FirebaseManager>();
        PV = GetComponent<PhotonView>();
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

    IEnumerator LoadXPAndKills()
    {
        yield return new WaitUntil(predicate: () => firebase.hasFixedDependencies == true);

        StartCoroutine(firebase.LoadExperience());
        StartCoroutine(firebase.LoadKills());
    }

    public void CheckLevelUp(int _currentExperience, GameObject _empty)
    {
        Debug.Log(_currentExperience);

        if (_currentExperience >= 1) //JUST FOR PLAYTESTING! MAKE SURE TO REVERT BACK TO 20 AFTER PLAYTESTING
        {
            LevelUp();
            currentExperience = 0;
            StartCoroutine(firebase.UpdateExperience(0));

            _empty.SetActive(true);
            _empty.GetComponent<Animator>().Play("LevelUpAnimation", 0, 0f);
            StartCoroutine(nameof(DisableLevelUpAnimation), _empty);
        }

        
    }


    private IEnumerator DisableLevelUpAnimation(GameObject __empty)
    {
        yield return new WaitForSeconds(2.5f);
        __empty.SetActive(false);
    }

    public void LevelUp()
    {
        currentLevel++;
        StartCoroutine(firebase.UpdateKills(currentLevel));

        if (PV.IsMine)
        {
            Hashtable hash = new();
            hash.Add("playerLevel", currentLevel);
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        }
    }

    public void AddExperiencePoints()
    {
        currentExperience++;
        StartCoroutine(firebase.UpdateExperience(currentExperience));
    }
}
