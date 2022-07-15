using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathEffectCustomize : MonoBehaviour
{
    public int deathEffectColorKey = 1;

    // Start is called before the first frame update
    void Start()
    {
        PlayerPrefs.SetInt("DeathEffectColor", 1);
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(PlayerPrefs.GetInt("DeathEffectColor"));
        Debug.Log(deathEffectColorKey);
    }

    public void ChangeToRed()
    {
        deathEffectColorKey = 1;
        PlayerPrefs.SetInt("DeathEffectColor", deathEffectColorKey);
    }
    public void ChangeToBlue()
    {
        deathEffectColorKey = 2;
        PlayerPrefs.SetInt("DeathEffectColor", deathEffectColorKey);
    }
    public void ChangeToGreen()
    {
        deathEffectColorKey = 3;
        PlayerPrefs.SetInt("DeathEffectColor", deathEffectColorKey);
    }
    public void ChangeToBlack()
    {
        deathEffectColorKey = 4;
        PlayerPrefs.SetInt("DeathEffectColor", deathEffectColorKey);
    }
}
