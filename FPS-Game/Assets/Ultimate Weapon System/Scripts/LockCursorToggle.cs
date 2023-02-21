using UnityEngine;
using System.Collections;

public class LockCursorToggle : MonoBehaviour{

	void Update(){
		if(Input.GetKeyDown(KeyCode.C)){
			if(Cursor.lockState == CursorLockMode.Locked){
				Cursor.visible = true;
				Cursor.lockState = CursorLockMode.None;
			}
			else{
				Cursor.visible = false;
				Cursor.lockState = CursorLockMode.Locked;
			}
		}
	}
}