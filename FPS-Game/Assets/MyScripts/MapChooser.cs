using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapChooser : MonoBehaviour
{
    [SerializeField] GameObject mapChooseContinueButton;
    public string mapName;

    public void ClearRoom()
    {
        mapName = "";
        mapChooseContinueButton.SetActive(false);
    }

    public void ChooseMap(string _mapName)
    {
        mapName = _mapName;
        mapChooseContinueButton.SetActive(true);
    }
}
