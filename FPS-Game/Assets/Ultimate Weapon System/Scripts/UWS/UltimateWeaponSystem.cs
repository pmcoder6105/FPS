//COPYRIGHT Nevaran --- 2015 All Rights Recieved
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UltimateWeaponSystem : MonoBehaviour{
	[Header("Other-----------------------------------------------------------------------------------------------------------------------------")]
	[Tooltip("Animator component that will be used for playing animations.")]
	public Animator animatorC;
	[Tooltip("The Audio Source component that will be used for playing sounds.")]
	public AudioSource audioSource;

	[System.Serializable]
	public class aimDownSights{
		[Tooltip("If true: The player will be able to use the aim system.")]
		public bool canAim = true;
		[Tooltip("Which key press makes the player aim with the weapon.")]
		public KeyCode aimKey = KeyCode.Mouse1;
		[Tooltip("How fast does the object move to the aim position.")]
		public float aimSpeed = 10.0f;
		[Tooltip("The aim object that will be moved around when aiming.")]
		public Transform aimObject;
		[Tooltip("The aim position(Note: You must manually setup the value to fit the weeapon).")]
		public Vector3 aimPosition;
	}
	[Tooltip("Aiming locally the weapon.")]
	public aimDownSights AimDownSights = new aimDownSights();

	private Vector3 defaultAimPosition;

	[Header("Ammunition-----------------------------------------------------------------------------------------------------------------------------")]
	[Tooltip("If true: The weapon will never need to reload nor run out of ammunition (Note: This makes the 'Ammo Info Type' to change to an infinite symbol).")]
	public bool infiniteAmmo = false;
	[Tooltip("The amount of ammunition that can be put in the weapon.")]
	public int clip = 30;
	[Tooltip("The amount of ammunition left in the weapon.")]
	public int clipLeft = 30;
	[Tooltip("The current amount of backup ammunition the player has.")]
	public int ammoStash = 90;
	[Tooltip("The maximum amount of backup ammunition the player can store.")]
	public int maxAmmoStash = 180;

	[Space(15)]
	[Tooltip("Optional: Text UI that where it will write the ammunition information in.")]
	public Text ammoInfo;
	public enum ammoInfoType{
		Full,
		Minimum,
		Partial
	}
	[Tooltip("Full: Clip Left + Max + Stash Left + Max" +
	         "\n\nMinimum: Clip Left + Stash Left" +
	         "\n\nPartial: Clip Left + Stash Left + Max")]
	public ammoInfoType AmmoInfoType;

	[Space(15)]
	[Tooltip("Optional: What will the object that fires from the shell escape position. Make sure the shell has a rigidbody.")]
	public Transform bulletShell;
	[Tooltip("Where will the 'shell' object spawn from. Position where it will be pushed from towards the shell escape's upwards local axis.")]
	public Transform bulletShellEscape;
	[Tooltip("How long does each shell last until it is removed. Keep this value above zero.")]
	public float shellLifespan = 10.0f;

	[Header("Firing-----------------------------------------------------------------------------------------------------------------------------")]
	[Tooltip("If true: Firing the weapon will continue if the button is held down.")]
	public bool automatic = true;

	[Space(15)]
	[Tooltip("If true: The weapon will have a charge and discharge stage.")]
	public bool gatling = false;
	[Tooltip("If true: The weapon sound will loop instead of starting all over again per shot.")]
	public bool loopingSound = false;
	[Tooltip("Optional: Sound effect before the weapon starts firing(spinup time).")]
	public AudioClip gatlingCharge;
	[Tooltip("Optional: Sound effect after the weapon finished firing.")]
	public AudioClip gatlingDischarge;
	[Tooltip("Optional: The object that will be rotated when firing gatling.")]
	public Transform rotator;
	[Tooltip("The local direction the rotator object will rotate, if any")]
	public Vector3 rotatorDir = Vector3.up;

	[Space(15)]
	[Tooltip("Key needed to be pressed to fire.")]
	public KeyCode fireKey = KeyCode.Mouse0;

	[System.Serializable]
	public class raycastFire{
		[Tooltip("If true: The raycast projectile system will be used instead of the object-based(default) one.")]
		public bool raycastFiring = false;
		
		[Tooltip("Which objects can the ray hit(their layer).")]
		public LayerMask raycastMask = -1;
		
		[Tooltip("The function name that the raycast will call upon a hit.")]
		public string damageName = "ApplyDamage";
		[Tooltip("Damage value sent to the defined function name.")]
		public float rayDamage = 10.0f;
		[Tooltip("How far does the raycast go from the fire position")]
		public float rayRange = 100.0f;
	}
	[Tooltip("Simple raycast projectile system-- Instant and much less performance cost compared to object-based projectiles.")]
	[Space(15)]
	public raycastFire RaycastFire = new raycastFire();

	[Space(15)]
	[Tooltip("Optional: An object containing the firing effects that are used per shot(Example: muzzle light and muzzle renderer).")]
	public GameObject muzzle;
	[Tooltip("If true: The muzzle object will rotate on the forward axis. This improves visual quality in most situations.")]
	public bool muzzleRotates = true;
	[Tooltip("If true: The muzzle game object will change it's position to the next fire position.")]
	public bool muzzleFollowsFire = false;

	[Space(15)]
	[Tooltip("Optional: The firing position(s) of the weapon. If its not set, the default firing position is the object the script is attached to. (Note: position fired is always on the forward axis)")]
	public Transform[] firePoint = new Transform[1];
	[Tooltip("If true: The firing point order for each fire will be random instead.")]
	public bool randomFireOrder = false;
	[Tooltip("Animation played when firing")]
	public AnimationClip fireAnimation;
	[Tooltip("An array of all the sounds that will be played when fired. These sounds are played by random.")]
	public AudioClip[] fireSounds;

	[Space(15)]
	[Tooltip("Instantiated projectile for the object-based fire.")]
	public Transform projectile;

	[Space(15)]
	[Tooltip("Optional: Adds a delay to the firing response. If gatling is enabled, it will act as a charge and discharge time.")]
	public float fireDelay = 0.0f;
	[Tooltip("How many times it will fire per second.")]
	public float fireRate = 0.05f;
	[Tooltip("The amount of projectiles that will be spawned per shot.")]
	public int pelletCount = 1;
	[Tooltip("The accuracy(in degrees) for the firing of the weapon.")]
	public float accuracy = 1.0f;

	[Space(15)]
	[Tooltip("Amount of times fired per burst fire. If the value is set to 1, it will fire normally.")]
	public int burstFireAmount = 1;
	[Tooltip("The time between each shot while burst firing (Note: Keep this value above 0 if using burst firing).")]
	public float burstFireRate = 0.2f;
	
	[Header("Reloading-----------------------------------------------------------------------------------------------------------------------------")]
	[Tooltip("The defined key for the reloading")]
	public KeyCode reloadKey = KeyCode.R;

	[Space(15)]
	[Tooltip("If true: The player can interrupt the reloading during a state.")]
	public bool interruptible = false;
	[Tooltip("If 'Interruptible' variable is true: The interrupt key for stopping a reloading proccess(Note: It is recommended to not use the same key like the Reload Key).")]
	public KeyCode interruptKey = KeyCode.Q;
	[Tooltip("Optional: If interrupted the reloading, it will play this animation.")]
	public AnimationClip stateInterruptAnimation;

	[Space(15)]
	[Tooltip("Optional: A Text UI that will show the 'State Info' text variable during the defined state.")]
	public Text infoCurrentState;
	[Tooltip("Optional: The Image UI fill amount(from 0 to 1) that represents the progress of the needed key tap amount of the state key needed.")]
	public Image tappingFillAmount;

	[System.Serializable]
	public class reloadStates{
		[Tooltip("Optional/Inspector: This is used only to make the inspector's elements easier to manage")]
		public string StateName = "Default State";
		[TextArea(1, 3)]
		[Tooltip("The information that will be shown during the defined state in a UI Text object.")]
		public string stateInfo = "<Info Template>";//example: tell the player which key he needs to press, and how many times, to continue
		[Tooltip("Which key is needed to be pressed in order to continue the state. If 'Tap Amount' is set to 0, this is disabled.")]
		public KeyCode tapKey = KeyCode.R;
		[Tooltip("How many times does the set 'Tap Key' variable input needs to be pressed before state is completed")]
		public int tapAmount = 1;

		[Space(15)]
		[Tooltip("If true: 'State Time' value is ignored and uses the State Animation's lenght instead.")]
		public bool AnimationTime = true;
		[Tooltip("Optional: The animation that will be played during the state. 'Animation C' Component needs to be set.")]
		public AnimationClip stateAnimation;
		[Tooltip("If 'Animation Time' is false: Used for waiting X amount of seconds before the state is complete.")]
		public float stateTime = 1.0f;

		[Space(15)]
		[Tooltip("Optional: The audio that will be set and played at the start of the defined state. This requires 'AudioSource' variable to be set.")]
		public AudioClip sfxClipStart;
		[Tooltip("Optional: The audio delay time(in seconds) for the 'Sfx Clip Start' variable.")]
		public float sfxStartDelay = 0.0f;
		[Tooltip("Optional: The audio that will be set and played at the end of the defined state. This requires 'AudioSource' variable to be set.")]
		public AudioClip sfxClipEnd;
		[Tooltip("Optional: An array of scripts that will be disabled at the start of the defined state. They are always enabled again in the end of the state.")]
		public MonoBehaviour[] disabledComponents;
	}
	[Space(15)]
	public reloadStates[] ReloadStates = new reloadStates[1];

	private int cState = 0;//the current state that will be played(goes +1 for each completed state)
	[HideInInspector]
	public bool firing = false;//is the weapon firing(gatling charge and discharge do not count as firing)
	[HideInInspector]
	public bool reloading = false;//this is true when a state is in progress
	private int tapCount = 0;//how many times a button is pressed per state
	private float nextFire = 0.0f;//in how many seconds will the next fire state be allowed
	[HideInInspector]
	public bool burstFiring = false;//is burst fire in progress?
	
	private float m_LastFrameShot = -1;//used for muzzle activity
	private float rotatorStart;
	private int nextFirePoint = 0;//the next fire point for the projectile
	private bool gatlingStarted = false;//has gatling fire started, if enabled

	public static UltimateWeaponSystem control;

	void Awake(){
		control = this;
	}

	//Start clean
	void Start(){
		if(firePoint.Length < 1){//if no fire point is defined, default to the transform that the script is attarched to
			firePoint = new Transform[1];
			firePoint[0] = transform;
		}
		else{
			if(!firePoint[0])
				firePoint[0] = transform;
		}

		if(infiniteAmmo && clip < 1)//just in case, set an ammunition of 1 if the clip is set to 0 or lower
			clip = 1;

		if(infoCurrentState)
			infoCurrentState.text = "";
		if(tappingFillAmount)
			tappingFillAmount.fillAmount = 0.0f;

		defaultAimPosition = AimDownSights.aimObject.localPosition;
	}

	//Used for inputs
	void Update(){
		if(!WeaponSwitcher.control || WeaponSwitcher.control && !WeaponSwitcher.control.animationC.isPlaying){
			if(reloading){
				if(interruptible && Input.GetKeyDown(interruptKey)){
					StopCoroutine("ReloadCycle");
					if(stateInterruptAnimation)
						animatorC.Play(stateInterruptAnimation.name, 0, 0);
					ResetCycle();
				}
			}
			else{
				if(!burstFiring){
					if(nextFire <= 0 && clipLeft > 0){
						if(automatic){
							if(Input.GetKey(fireKey)){
								if(burstFireAmount <= 1)
									StartCoroutine("Fire");
//									Fire();
								else
									StartCoroutine("BurstFire");
							}
						}
						else{
							if(Input.GetKeyDown(fireKey)){
								if(burstFireAmount <= 1)
									StartCoroutine("Fire");
//									Fire();
								else
									StartCoroutine("BurstFire");
							}
						}
					}
					if(Input.GetKeyDown(reloadKey) && clipLeft < clip && ammoStash > 0 && !infiniteAmmo){
						if(ReloadStates.Length > 0){
							StartCoroutine("ReloadCycle");

							StopCoroutine("AimSights");
							StartCoroutine("AimSights", false);

							reloading = true;
						}
					}
				}
				if(Input.GetKeyDown(AimDownSights.aimKey)){
					StopCoroutine("AimSights");
					StartCoroutine("AimSights", true);
				}
				else if(Input.GetKeyUp(AimDownSights.aimKey)){
					StopCoroutine("AimSights");
					StartCoroutine("AimSights", false);
				}
			}
			if (firing) {
				if (Input.GetKeyUp (fireKey) || clipLeft <= 0) {
					StopCoroutine ("Fire");
					StopFiring ();
				}
			}
		}

		if(m_LastFrameShot >= Time.frameCount - 3){
			if(muzzle)
				muzzle.SetActive(true);
		}
		else{
			if(muzzle)
				muzzle.SetActive(false);
		}
	}

	void FixedUpdate(){
		if(rotator && rotatorStart > 0.0f){
			rotator.Rotate(rotatorDir * (60 / fireRate) * rotatorStart * Time.fixedDeltaTime);
		}
		if(firing){
			if(rotatorStart < 1.0f){
				rotatorStart = Mathf.Lerp(rotatorStart, 1.0f, Time.fixedDeltaTime / fireDelay);
			}
		}
		else{
			if(rotatorStart > 0.0f){
				rotatorStart = Mathf.Lerp(rotatorStart, 0.0f, Time.fixedDeltaTime / (fireDelay / 2));
			}
		}

		if(nextFire > 0)
			nextFire -= Time.fixedDeltaTime;
	}

	IEnumerator AimSights(bool aiming){
		if(aiming){
			while(AimDownSights.aimObject.localPosition != AimDownSights.aimPosition){
				AimDownSights.aimObject.localPosition = Vector3.Slerp(AimDownSights.aimObject.localPosition, AimDownSights.aimPosition, Time.fixedDeltaTime * AimDownSights.aimSpeed);
				yield return new WaitForFixedUpdate();
			}
			AimDownSights.aimObject.localPosition = AimDownSights.aimPosition;
		}
		else{
			while(AimDownSights.aimObject.localPosition != defaultAimPosition){
				AimDownSights.aimObject.localPosition = Vector3.Slerp(AimDownSights.aimObject.localPosition, defaultAimPosition, Time.fixedDeltaTime * AimDownSights.aimSpeed);
				yield return new WaitForFixedUpdate();
			}
			AimDownSights.aimObject.localPosition = defaultAimPosition;
		}
	}

	IEnumerator Fire(){
		firing = true;
		nextFire = fireRate;
		if(gatling && !gatlingStarted){
			gatlingStarted = true;
			if (gatlingCharge) {
				audioSource.clip = gatlingCharge;
				audioSource.Play ();
			}
		}
		yield return new WaitForSeconds(fireDelay);

		if(muzzle && muzzleRotates){
			muzzle.transform.Rotate(Vector3.forward * 29);//29 is an arbitrary variables, personal liking on how the muzzle rotates with this value
		}

		for(int i = 0; i < pelletCount; i++){
			if(firePoint.Length > 1){//dont have pointless cpu action if we dont have more than 1 fire point
				if(randomFireOrder){
					nextFirePoint = Random.Range(0, firePoint.Length-1);
				}
				else{
					nextFirePoint++;
					if(nextFirePoint > firePoint.Length-1)
						nextFirePoint = 0;
				}
				if(muzzleFollowsFire){
					muzzle.transform.position = firePoint[nextFirePoint].position;
				}
			}
			if(RaycastFire.raycastFiring){
				RaycastHit hit;
				if(Physics.Raycast(firePoint[nextFirePoint].position, firePoint[nextFirePoint].forward, out hit, RaycastFire.rayRange, RaycastFire.raycastMask)){
					hit.transform.gameObject.SendMessageUpwards(RaycastFire.damageName, RaycastFire.rayDamage, SendMessageOptions.DontRequireReceiver);
				}
			}
			else{
				Quaternion rotation = Quaternion.Euler(Random.insideUnitSphere * accuracy);
				firePoint[nextFirePoint].localRotation = rotation;
				Instantiate(projectile, firePoint[nextFirePoint].position, firePoint[nextFirePoint].rotation);
			}
		}
		firePoint[nextFirePoint].localRotation = Quaternion.identity;

		ShellSpawn ();//spawn a shell, if any

		if(fireAnimation){
			animatorC.Play(fireAnimation.name, 0, 0);
		}
		if(fireSounds.Length > 0){
			audioSource.clip = fireSounds[Random.Range(0, fireSounds.Length)];
			if(loopingSound){
				audioSource.loop = true;
				if(!audioSource.isPlaying){
					audioSource.Play();
				}
			}
			else{
				audioSource.Play();
			}
		}


		if(!infiniteAmmo){
			clipLeft--;
		}
		m_LastFrameShot = Time.frameCount;

		UpdateAmmo();

		if (!automatic)
			StopFiring ();
	}

	IEnumerator BurstFire(){
		burstFiring = true;
		int shotsDone = 0;//how many shots have been fired during a burst fire cycle
		while(shotsDone < burstFireAmount && clipLeft > 0){
//			Fire();
			StartCoroutine("Fire");
			yield return new WaitForSeconds(burstFireRate);
			yield return null;
			shotsDone++;
		}
		burstFiring = false;
	}

	IEnumerator ReloadCycle(){
		yield return 0;//wait a frame so the update keys dont affect tap count if they are the same key

		tapCount = 0;
		for(int i = 0; i < ReloadStates[cState].disabledComponents.Length; i++){
			ReloadStates[cState].disabledComponents[i].enabled = false;
		}
		if(infoCurrentState)
			infoCurrentState.text = ReloadStates[cState].stateInfo;

		if(ReloadStates[cState].tapKey == KeyCode.None || ReloadStates[cState].tapAmount == 0){
			//If no tap key or tap amount is set
		}
		else{
			while(tapCount < ReloadStates[cState].tapAmount){
				if(Input.GetKeyDown(ReloadStates[cState].tapKey)){
					tapCount++;
					if(tappingFillAmount && ReloadStates[cState].tapAmount > 0)
						tappingFillAmount.fillAmount = ((float)tapCount / (float)ReloadStates[cState].tapAmount);
				}
				yield return null;
			}
			if(infoCurrentState)//hide the state info if we finished the key tapping process
				infoCurrentState.text = "";
		}

		if(ReloadStates[cState].stateAnimation)
			animatorC.Play(ReloadStates[cState].stateAnimation.name, 0, 0);
		if(ReloadStates[cState].sfxClipStart){
			StartCoroutine(DelayedSound());
		}

		if(ReloadStates[cState].AnimationTime){
			yield return new WaitForSeconds(ReloadStates[cState].stateAnimation.length);
		}
		else{
			yield return new WaitForSeconds(ReloadStates[cState].stateTime);
		}

		if(ReloadStates[cState].sfxClipEnd){
			audioSource.clip = ReloadStates[cState].sfxClipEnd;
			audioSource.Play();
		}

		for(int i = 0; i < ReloadStates[cState].disabledComponents.Length; i++){
			ReloadStates[cState].disabledComponents[i].enabled = true;
		}
		if(cState < ReloadStates.Length-1){
			cState++;
			ResetState();
			StartCoroutine("ReloadCycle");//Move to the next state if we havent reached the end yet
		}
		//Finish the reloading states and sets everything back to default
		else{
			int ammoToFull = clip - clipLeft;//adding to clip and removing from stash amount
			if(ammoStash > ammoToFull){
				
			}
			else{
				ammoToFull = ammoStash;
			}
			ammoStash -= ammoToFull;
			clipLeft += ammoToFull;

			UpdateAmmo();

			ResetCycle();
		}
	}

	void ShellSpawn(){
		if(bulletShell && bulletShellEscape){
			Transform cloneShell = Instantiate(bulletShell, bulletShellEscape.position, bulletShellEscape.rotation) as Transform;
			Rigidbody cloneShellRigid = cloneShell.GetComponent<Rigidbody> ();

			//pushes the clone shell upwards Y on the axis of the shell escape target
			cloneShellRigid.AddForce(bulletShellEscape.up * Random.Range(3.0f, 5.0f), ForceMode.VelocityChange);
			cloneShellRigid.AddTorque(Random.Range(-30, 30), Random.Range(-30, 30), Random.Range(-30, 30));

			cloneShell.gameObject.SetActive(true);
			cloneShell.parent = null;
			Destroy(cloneShell.gameObject, shellLifespan);
		}
	}

	void StopFiring(){//used to disable some things easier
		if(loopingSound){
			audioSource.Stop();
			audioSource.loop = false;
		}
		if(gatling && gatlingDischarge && firing){
			audioSource.clip = gatlingDischarge;
			audioSource.Play();
		}
		gatlingStarted = false;
		firing = false;
	}

	//Used for the delayed sound play in a state, if any
	IEnumerator DelayedSound(){
		yield return new WaitForSeconds(ReloadStates[cState].sfxStartDelay);
		audioSource.clip = ReloadStates[cState].sfxClipStart;
		audioSource.Play();
	}

	void OnEnable(){
		UpdateAmmo();

		StopCoroutine("AimSights");
		AimDownSights.aimObject.localPosition = defaultAimPosition;

		rotatorStart = 0.0f;
	}
	
	void OnDisable(){
		ResetCycle();//resets to default if the script suddently gets destroyed/disabled during a state cylce
		burstFiring = false;
		StopCoroutine("BurstFire");
		if(muzzle)
			muzzle.SetActive(false);
	}

	/// <summary>
	/// Updates the ammunition UI and limits.
	/// </summary>
	public void UpdateAmmo(){
		if(clipLeft > clip)
			clipLeft = clip;
		if(ammoStash > maxAmmoStash)
			ammoStash = maxAmmoStash;

		if(ammoInfo){
			if(infiniteAmmo){
				ammoInfo.text = "∞";//should be the infinite symbol
			}
			else{
				switch(AmmoInfoType){
				case ammoInfoType.Full:
					ammoInfo.text = clipLeft + "/" + clip + "\n\t" + ammoStash + "/" + maxAmmoStash;
					break;
				case ammoInfoType.Minimum:
					ammoInfo.text = clipLeft + "\n\t" + ammoStash;
					break;
				case ammoInfoType.Partial:
					ammoInfo.text = clipLeft + "\n\t" + ammoStash + "/" + maxAmmoStash;
					break;
				}
			}
		}
	}

	/// <summary>
	/// Resets the state. Used when a state finishes and another one is queued.
	/// </summary>
	public void ResetState(){
		//s
		tapCount = 0;
		//r
		//i
		if(tappingFillAmount)
			tappingFillAmount.fillAmount = 0.0f;
	}

	/// <summary>
	/// Resets both the state and the cycle.
	/// </summary>
	public void ResetCycle(){
		cState = 0;
		tapCount = 0;
		reloading = false;
		if(infoCurrentState)
			infoCurrentState.text = "";
		if(tappingFillAmount)
			tappingFillAmount.fillAmount = 0.0f;
	}
}
