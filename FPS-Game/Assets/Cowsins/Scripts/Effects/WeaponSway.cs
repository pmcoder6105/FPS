/// <summary>
/// This script belongs to cowsins™ as a part of the cowsins´ FPS Engine. All rights reserved. 
/// </summary>
using cowsins; 
using UnityEngine;

public class WeaponSway : MonoBehaviour 
{
	[Header ("Position")]
	[SerializeField] private float amount = 0.02f;

	[SerializeField] private float maxAmount = 0.06f ;

	[SerializeField] private float smoothAmount = 6f;


	[Header ("Tilting")]
	[SerializeField] private float tiltAmount = 4f;

	[SerializeField] private float maxTiltAmount = 5f;

	[SerializeField] private float smoothTiltAmount = 12f;

	private WeaponController player;

	private Vector3 initialPosition;

	private Quaternion initialRotation;

	private float InputX;

	private float InputY;

	private float playerMultiplier;

	private void Start(){
		initialPosition = transform.localPosition;
		initialRotation = transform.localRotation;
		player = GameObject.Find("Player").GetComponent<WeaponController>();
	}

	private void Update(){
		if (!PlayerStats.Controllable) return; 
		CalculateSway();
		MoveSway();
		TiltSway();
	}
	private void CalculateSway(){
		InputX = -InputManager.mousex /10 - 5* InputManager.controllerx;
		InputY = -InputManager.mousey/10 - 2 * InputManager.controllery;

		if (player.isAiming) playerMultiplier = 5f;
		else playerMultiplier = 1f;
	}

	private void MoveSway(){

		float moveX = Mathf.Clamp(InputX * amount, -maxAmount, maxAmount) / playerMultiplier;	
		float moveY = Mathf.Clamp(InputY * amount, -1, 1) / playerMultiplier;

		Vector3 finalPosition = new Vector3(moveX, moveY, 0);

		transform.localPosition = Vector3.Lerp(transform.localPosition, finalPosition + initialPosition, Time.deltaTime * smoothAmount * playerMultiplier);
		
	}

	private void TiltSway()
	{
		float moveX = Mathf.Clamp(InputX * tiltAmount, -maxTiltAmount, maxTiltAmount) / playerMultiplier;

		Quaternion finalRotation = Quaternion.Euler(0,0, moveX);

		transform.localRotation = Quaternion.Lerp(transform.localRotation, finalRotation * initialRotation, Time.deltaTime * smoothTiltAmount * playerMultiplier);
	}

}

