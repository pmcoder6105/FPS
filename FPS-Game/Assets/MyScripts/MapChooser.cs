using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapChooser : MonoBehaviour
{
    public int mapChosen = 0;
    [SerializeField] GameObject mapChooseContinueButton;
    public string mapName;

    public void OpposingPills()
    {
        mapChosen = 1;
        mapName = "OpposingBeans";
        mapChooseContinueButton.SetActive(true);
    }
    public void PillArena()
    {
        mapChosen = 2;
        mapName = "BeanArena";
        mapChooseContinueButton.SetActive(true);
    }
    public void Pillars()
    {
        mapChosen = 3;
        mapName = "PillarsOfMadness";
        mapChooseContinueButton.SetActive(true);
    }
    public void Trials()
    {
        mapChosen = 4;
        mapName = "BeanTrials";
        mapChooseContinueButton.SetActive(true);
    }
    public void UnderGround()
    {
        mapChosen = 5;
        mapName = "UndergroundPyramid";
        mapChooseContinueButton.SetActive(true);
    }
    public void Sky()
    {
        mapChosen = 6;
        mapName = "Sky-Beans";
        mapChooseContinueButton.SetActive(true);
    }

    public void FightingGround()
    {
        mapChosen = 7;
        mapName = "BeanFightingGround";
        mapChooseContinueButton.SetActive(true);
    }

    public void CloseCombat()
    {
        mapChosen = 8;
        mapName = "CloseBeanCombat";
        mapChooseContinueButton.SetActive(true);
    }

    public void PillBox()
    {
        mapChosen = 9;
        mapName = "PillBox";
        mapChooseContinueButton.SetActive(true);
    }

    public void ClearRoom()
    {
        mapChosen = 0;
        mapName = "";
        mapChooseContinueButton.SetActive(false);
    }
}
