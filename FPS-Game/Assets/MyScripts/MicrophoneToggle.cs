using UnityEngine;
using UnityEngine.UI;

public class MicrophoneToggle : MonoBehaviour
{
    // Reference to the button that will be used to toggle the microphone
    //public Button toggleButton;

    // String to store the name of the active microphone
    private string activeMicrophone;

    // Boolean that tracks whether the microphone is currently active
    private bool microphoneIsActive = false;

    void Start()
    {
        // Get the name of the default microphone
        activeMicrophone = Microphone.devices[0];

        // Add a listener to the button's OnClick event
        //toggleButton.onClick.AddListener(ToggleMicrophone
    }

    public void ToggleMicrophone()
    {
        if (microphoneIsActive)
        {
            // Stop the microphone and update the boolean
            Microphone.End(activeMicrophone);
            microphoneIsActive = false;
        }
        else
        {
            // Start the microphone and update the boolean
            Microphone.Start(activeMicrophone, true, 1, 44100);
            microphoneIsActive = true;
        }
    }
}
