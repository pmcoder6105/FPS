using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenVisualBeanOMeterReward : MonoBehaviour
{
    [SerializeField] MainMenuVisualReward[] reward;

    public void OpenReward(string menuName)
    {
        for (int i = 0; i < reward.Length; i++)
        {
            if (reward[i].rewardName == menuName)
            {
                reward[i].Open();
            }
            else if (reward[i].open)
            {
                CloseReward(reward[i]);
            }
        }
    }

    public void OpenReward(MainMenuVisualReward menu)
    {
        for (int i = 0; i < reward.Length; i++)
        {
            if (reward[i].open)
            {
                CloseReward(reward[i]);
            }
        }
        menu.Open();
    }

    public void CloseReward(MainMenuVisualReward reward)
    {
        reward.Close();
    }
}
