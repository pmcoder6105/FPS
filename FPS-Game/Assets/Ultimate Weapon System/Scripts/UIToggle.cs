using UnityEngine;
using UnityEngine.UI;

public class UIToggle : MonoBehaviour{
	public GameObject uiWindow;
	public KeyCode toggleKey = KeyCode.Q;

	void Update(){
		if (Input.GetKeyDown (toggleKey)) {
			if (uiWindow.activeInHierarchy) {
				uiWindow.SetActive (false);
			}
			else {
				uiWindow.SetActive (true);
			}
		}
	}
}