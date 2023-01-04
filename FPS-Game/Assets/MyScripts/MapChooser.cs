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
        mapChosen = 2;
        mapName = "OpposingBeans";
        mapChooseContinueButton.SetActive(true);
    }
    public void PillArena()
    {
        mapChosen = 3;
        mapName = "BeanArena";
        mapChooseContinueButton.SetActive(true);
    }
    public void Pillars()
    {
        mapChosen = 4;
        mapName = "PillarsOfMadness";
        mapChooseContinueButton.SetActive(true);
    }
    public void Trials()
    {
        mapChosen = 5;
        mapName = "Portals";
        mapChooseContinueButton.SetActive(true);
    }
    public void UnderGround()
    {
        mapChosen = 6;
        mapName = "UndergroundPyramid";
        mapChooseContinueButton.SetActive(true);
    }
    public void Sky()
    {
        mapChosen = 7;
        mapName = "Sky-Beans";
        mapChooseContinueButton.SetActive(true);
    }

    public void FightingGround()
    {
        mapChosen = 8;
        mapName = "BeanFightingGround";
        mapChooseContinueButton.SetActive(true);
    }

    public void CloseCombat()
    {
        mapChosen = 9;
        mapName = "FunArena";
        mapChooseContinueButton.SetActive(true);
    }

    public void PillBox()
    {
        mapChosen = 10;
        mapName = "Dropper";
        mapChooseContinueButton.SetActive(true);
    }

    public void ClearRoom()
    {
        mapChosen = 0;
        mapName = "";
        mapChooseContinueButton.SetActive(false);
    }
}
