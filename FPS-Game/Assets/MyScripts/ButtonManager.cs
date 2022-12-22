using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
    private static ButtonManager _singleton;

    private Button[] buttons;

    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Singleton = this;
        DontDestroyOnLoad(this.gameObject);
        Button[] buttons = FindObjectsOfType<Button>();

        foreach (Button item in buttons)
        {
            if (item.gameObject.GetComponent<Outline>() != null)
                return;
            Debug.Log(item.gameObject);
            item.gameObject.AddComponent<Outline>();
            item.gameObject.GetComponent<Outline>().effectDistance = new Vector2(-7.5f, -5);
            if (ColorUtility.TryParseHtmlString("#5B5B5B", out Color outlineColor))
                item.gameObject.GetComponent<Outline>().effectColor = outlineColor;
        }
    }

    public static ButtonManager Singleton
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
                Debug.Log($"{nameof(ButtonManager)} instance already exists, destroying object!");
                Destroy(value.gameObject);
            }
            Debug.Log("Singleton called");
        }
    }
}
