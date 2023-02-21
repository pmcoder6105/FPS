//Stripped version of my MouseLook script, included in the Modern First Person Shooter Kit asset available at the asset store!
using UnityEngine;
using System.Collections;

public class MouseControl : MonoBehaviour{
	public float sensitivity = 5.0f;

	public float yMin = 90.0f, yMax = 90.0f;
	public float xMin = 360.0f, xMax = 360.0f;

	private float rotationX = 0.0f;
	private float rotationY = 0.0f;

	private Quaternion originalRotation;

	public Transform Player;//Player object for rotating the X axis

	void Start(){
		originalRotation = transform.localRotation;
	}

	void Update(){
		Quaternion yQuaternion;
		Quaternion xQuaternion;

		rotationX += Input.GetAxis("Mouse X") * sensitivity * Time.fixedDeltaTime * 60;
		rotationY += Input.GetAxis("Mouse Y") * sensitivity * Time.fixedDeltaTime * 60;

		rotationX = ClampAngle(rotationX, -xMin, xMax);
		rotationY = ClampAngle(rotationY, -yMin, yMax);

		xQuaternion = Quaternion.AngleAxis(rotationX, Vector3.up);
		yQuaternion = Quaternion.AngleAxis(rotationY, Vector3.left);

		transform.localRotation = originalRotation * yQuaternion;
		if(Player)
			Player.localRotation = originalRotation * xQuaternion;
	}

	static float ClampAngle(float angle, float min, float max){
		if(angle < -360){
			angle += 360;
		}
		if(angle > 360){
			angle -= 360;
		}
		return Mathf.Clamp(angle, min, max);
	}
}