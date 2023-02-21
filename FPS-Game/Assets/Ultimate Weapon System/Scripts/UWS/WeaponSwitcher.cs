//COPYRIGHT Nevaran --- 2015 All Rights Recieved
using UnityEngine;
using System.Collections;

public class WeaponSwitcher : MonoBehaviour{
	[Tooltip("Which key is used to holster the currently drawn weapon")]
	public KeyCode holsterKey = KeyCode.H;
	
	[Tooltip("If true: The player will start with a drawn weapon instead of holstered.")]
	public bool startDrawn = false;
	[Tooltip("The current weapon(element number) that will start as default with.\n0 = first weapon | -1 = holstered")]
	public int current = 0;

	private int previousCurrent;//this is used for activating the switching -- if current is not equal to this value, it will do a switch
	private int holsterCurrent;//the weapon we holstered from
	
	[Space(15)]
	[Tooltip("How much time the holster key needs to be held to holster the current weapon.")]
	public float holsterTime = 1.0f;

	[HideInInspector]
	public float pressTime;//the amount of time the holster key has been held
	[HideInInspector]
	public bool syncHolsterDone = true;//toggling aligner
	
	[Space(15)]
	[Tooltip("If true: The motion system will be active.")]
	public bool hasMotion = true;
	[Tooltip("Which object is affected by the motion.")]
	public Transform motionTarget;
	[Tooltip("How fast does the motion rotation speed is.")]
	public float motionSpeed = 10.0f;
	[Tooltip("The direction, and speed of which the motion will go towards. Setting a value to 0 will disable the motion for that direction axis.")]
	public Vector3 motionDirection = new Vector3(1, 1, 1);
	[Tooltip("The euler rotation limit of the 'Motion Target' object(from zero).")]
	public Vector3 motionAngleLimit = new Vector3(15, 15, 5);
	
	[Space(15)]
	[Tooltip("The animation component which has the draw and holster animations.")]
	public Animation animationC;
	[Tooltip("All weapons used in the 'Weapon Switcher' script. There is always ONE active, and otherse are disabled")]
	public GameObject[] weapons;
	
	[Tooltip("If the value is more than 0, it will lock the ability to change weapons or holster the current one.")]
	public float actionCooldown = 0.3f;//time before you can do an action(switch weapons, ect.)
	
	public static WeaponSwitcher control;//allows for public variables and functions to be accessed from any other script
	
	void Awake(){
		control = this;
	}
	
	void Start(){
		if(startDrawn){
			previousCurrent = current;
			animationC.Stop();
			animationC.Play("Draw");
			actionCooldown = animationC["Draw"].length;
		}
		else{
			current = -1;
			previousCurrent = current;
			animationC.Play("Holster");
			actionCooldown = animationC["Holster"].length;
		}
		for(int i = 0; i < weapons.Length; i++){
			if(weapons[i]){
				if(current == i){
					weapons[i].SetActive(true);
				}
				else{
					weapons[i].SetActive(false);
				}
			}
		}
	}
	
	void OnGUI(){
		if(actionCooldown <= 0){
			Event e = Event.current;
		
			if(e.isKey && char.IsDigit (e.character)){
				int.TryParse(e.character.ToString(), out current);
				current--;//sync with array vlues
				current = Mathf.Clamp(current, 0, weapons.Length - 1);
			}
		}
	}
	
	private float XRot = 0.0f;
	private float YRot = 0.0f;
	private float ZRot = 0.0f;
	
	void Update(){
		if(UltimateWeaponSystem.control && UltimateWeaponSystem.control.reloading){//keep the switching locked untill we finished our weapon business
			actionCooldown = 1.0f;
		}
		if(actionCooldown <= 0){
			if(!UltimateWeaponSystem.control || 
				UltimateWeaponSystem.control && !UltimateWeaponSystem.control.burstFiring && !UltimateWeaponSystem.control.firing){

				if(Input.GetKeyDown(holsterKey) && syncHolsterDone){
					pressTime = Time.time;
					syncHolsterDone = false;
					if(current == -1)
						StartCoroutine(HolsterToggle());
				}
				if(Input.GetKey(holsterKey) && !syncHolsterDone && Time.time > pressTime + holsterTime){
					StartCoroutine(HolsterToggle());
				}
				//uncomment this if you want to draw current weapon if pressing the fire button
//				if(Input.GetKeyDown(UltimateWeaponSystem.control.fireKey) && current == -1){
//					StartCoroutine(HolsterToggle());
//				}
				if(Input.GetAxis("Mouse Scroll Wheel") > 0){
					current++;
					if(current > weapons.Length-1)
						current = 0;
				}
				else if(Input.GetAxis("Mouse Scroll Wheel") < 0){
					current--;
					if(current < 0)
						current = weapons.Length-1;
				}
			}
		}
		
		if(previousCurrent != current){
			previousCurrent = current;
			StartCoroutine(SwitchWeapons(current));
		}
	}
	
	void FixedUpdate(){
		if(actionCooldown > 0){
			actionCooldown -= Time.fixedDeltaTime;
		}

		//--------weapon motion--------
		if(hasMotion){
			//Note: The multiplied by 50 part is the update game time(default 0.02 which is 50 frames a second).
			//Change it to what is your game using if you want
			XRot += Input.GetAxis("Mouse X") * motionDirection.x * Time.fixedDeltaTime * 50;
			YRot += Input.GetAxis("Mouse Y") * motionDirection.y * Time.fixedDeltaTime * 50;
			ZRot += Input.GetAxis("Mouse X") * motionDirection.z * Time.fixedDeltaTime * 50;

			if(motionAngleLimit.x > 0){
				XRot = Mathf.Clamp(XRot, -motionAngleLimit.x, motionAngleLimit.x);
				if(XRot != 0.0f){
					XRot = Mathf.Lerp(XRot, 0, Time.fixedDeltaTime * motionSpeed);
				}
			}
			if(motionAngleLimit.y > 0){
				YRot = Mathf.Clamp(YRot, -motionAngleLimit.y, motionAngleLimit.y);
				if(YRot != 0.0f){
					YRot = Mathf.Lerp(YRot, 0, Time.fixedDeltaTime * motionSpeed);
				}
			}
			if(motionAngleLimit.z > 0){
				ZRot = Mathf.Clamp(ZRot, -motionAngleLimit.z, motionAngleLimit.z);
				if(ZRot != 0.0f){
					ZRot = Mathf.Lerp(ZRot, 0, Time.fixedDeltaTime * motionSpeed);
				}
			}
			motionTarget.localRotation = Quaternion.Lerp(motionTarget.localRotation, Quaternion.Euler(YRot, -XRot, ZRot), Time.fixedDeltaTime * motionSpeed);
		}
	}

	//For when the weapons arent attached to the player
	//	void LateUpdate(){
	//		for(int i = 0; i < weapoweapons.LengthnSize; i++){
	//			if(weapons[i]){
	//				weapons[i].transform.position = transform.position;
	//				weapons[i].transform.rotation = transform.rotation;
	//			}
	//		}
	//	}
	
	IEnumerator SwitchWeapons(int newWeapon){
		if(newWeapon == -1){
			for(int i = 0; i < weapons.Length; i++){
				if(weapons[i]){
					if(newWeapon == i){
						weapons[i].SetActive(true);
					}
					else{
						weapons[i].SetActive(false);
					}
				}
			}
		}
		else{
			animationC.Stop();
			animationC.Play("Holster");
			actionCooldown = animationC["Holster"].length;
			yield return new WaitForSeconds(animationC["Holster"].length);
			
			for(int i = 0; i < weapons.Length; i++){
				if(weapons[i]){
					if(newWeapon == i){
						weapons[i].SetActive(true);
					}
					else{
						weapons[i].SetActive(false);
					}
				}
			}
			
			animationC.Stop();
			animationC.Play("Draw");
			actionCooldown = animationC["Draw"].length;
		}
	}
	
	IEnumerator HolsterToggle(){
		if(current != -1){
			animationC.Stop();
			animationC.Play("Holster");
			actionCooldown = animationC["Holster"].length;
			holsterCurrent = current;
			yield return new WaitForSeconds(animationC["Holster"].length);
			current = -1;
			syncHolsterDone = true;
		}
		else{
			animationC.Stop();
			animationC.Play("Draw");
			actionCooldown = animationC["Draw"].length;
			current = holsterCurrent;
			syncHolsterDone = true;
		}
	}
}