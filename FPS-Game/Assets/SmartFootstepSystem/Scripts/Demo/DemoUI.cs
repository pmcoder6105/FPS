using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(SmartFootstepSystem))]
public class DemoUI : MonoBehaviour {

	private SmartFootstepSystem footstepManager;
	public Text onTerrain;
	public Text currentTexture;
	
	void Start () {
	footstepManager = GetComponent<SmartFootstepSystem>();
	}
	
	void Update () {
	onTerrain.text = OnTerrain();
	currentTexture.text = CurrentTexture();
	
	if(Input.GetKeyDown(KeyCode.R))
	Application.LoadLevel(Application.loadedLevel);
	}
	
	public void LoadFPS(){
	Application.LoadLevel("FirstPersonDemo");
	}
	
	public void LoadTPS(){
	Application.LoadLevel("ThirdPersonDemo");
	}
	
	public string OnTerrain(){
	return footstepManager.onTerrain.ToString();
	}
	
	public string CurrentTexture(){
		if(!footstepManager.currentTexture){
			return null;
		}
		else{
			return footstepManager.currentTexture.name;
		}
	}
}
