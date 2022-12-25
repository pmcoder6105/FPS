using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BadWordManager : MonoBehaviour
{
    InputField inFieldText;
    string myString;
    [SerializeField] string[] badWords = { "fuck", "shit", "cock", "tit", "pussy", "asshole", "cunt", "nigg", "stupid", "bastard", "idiot", "penis", "vagina", "dick", "turd", "crap", "retard", "jerk", "scum", "bitch"};


    // Start is called before the first frame update
    void Start()
    {
        inFieldText = GetComponent<InputField>();
    }

    public void ChangeName(string nameIn)
    {
        myString = nameIn;
        BadWordParser();
    }

    void BadWordParser()
    {
        for (int i = 0; i < badWords.Length; i++)
        {
            if (myString.ToLower().Contains(badWords[i]))
            {
                for (int j = 0; j < myString.Length; j++)
                {
                    if (myString.ToLower()[j] == badWords[i][0])
                    {
                        string temp = myString.Substring(j, badWords[i].Length);
                        if (temp.ToLower() ==  badWords[i])
                        {
                            myString = myString.Remove(j, badWords[i].Length);
                            if (myString != null)
                            {
                                inFieldText.text = myString.ToString();
                            }
                            else
                            {
                                inFieldText.text = "";
                            }
                            return;
                        }
                    }
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
