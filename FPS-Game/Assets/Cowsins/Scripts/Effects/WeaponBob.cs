/// <summary>
/// This script belongs to cowsins™ as a part of the cowsins´ FPS Engine. All rights reserved. 
/// </summary>
using UnityEngine;
using cowsins;

public class WeaponBob : MonoBehaviour
{
	[SerializeField] private float speed = 1f, distance = 1f;

	[SerializeField] private PlayerMovement PlayerMovement;

	private float _distance = 1f;

	private float timer, movement;

	private Vector3 midPoint;

	private Quaternion startRot;

	private Rigidbody Player;

	private float lerpSpeed;

	private void Start()
	{
		midPoint = transform.localPosition;
		startRot = transform.localRotation;
		Player = PlayerMovement.GetComponent<Rigidbody>();
	}

	private void Update()
	{
		if (!PlayerMovement.grounded && !PlayerMovement.wallRunning) return;
		GroundedMovement();
	}

	 private void GroundedMovement()
	 {
		_distance = distance * Player.velocity.magnitude / 1.5f * Time.deltaTime;
		speed = Player.velocity.magnitude / 1.5f * Time.deltaTime;
		Vector3 localPosition = transform.localPosition;
		Quaternion localRotation = transform.localRotation;

		if (Mathf.Abs(InputManager.x) == 0 && Mathf.Abs(InputManager.y) == 0)
		{
			timer = Mathf.Lerp(timer, 0, Time.deltaTime);
		}
		else
		{
			movement = Mathf.Sin(timer);
			timer += speed;
			if (timer > Mathf.PI * 2)
			{
				timer = timer - (Mathf.PI * 2);
			}
		}
		if (movement != 0)
		{
			float translateChange = movement * distance / 100;
			float totalAxes = Mathf.Abs(InputManager.x) + Mathf.Abs(InputManager.y);
			totalAxes = Mathf.Clamp(totalAxes, 0.0f, 1.0f);
			translateChange = totalAxes * translateChange;
			localPosition.y = midPoint.y + translateChange * 3;
			localPosition.z = midPoint.z + translateChange * 2;
			localPosition.x = startRot.x + translateChange * 2f;

		}
		else
		{
			localPosition.y = midPoint.y;
			localPosition.x = midPoint.x;
		}

			transform.localPosition = Vector3.Lerp(transform.localPosition, localPosition, Time.deltaTime * 10);
			transform.localRotation = Quaternion.Lerp(transform.localRotation, localRotation, Time.deltaTime * 10);
	 }
}