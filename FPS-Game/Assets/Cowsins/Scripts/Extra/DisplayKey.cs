#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif
using UnityEngine;
using TMPro; 

public class DisplayKey : MonoBehaviour
{
    public static PlayerActions inputActions;

    private void Awake()
    {
        if (inputActions == null)
        {
            inputActions = new PlayerActions();
            inputActions.Enable();
        }

    }
    private void Start() => GetComponent<TextMeshProUGUI>().text = inputActions.GameControls.Interacting.GetBindingDisplayString(); 
}
