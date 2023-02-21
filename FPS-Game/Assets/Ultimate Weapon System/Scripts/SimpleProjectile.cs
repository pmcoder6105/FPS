using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class SimpleProjectile : MonoBehaviour{
	public float speed = 10.0f;
	public float lifeSpan = 5.0f;

	void Start(){
		GetComponent<Rigidbody>().velocity = transform.forward * speed;
		Destroy(gameObject, lifeSpan);
	}

	void OnCollisionEnter(){
		GetComponent<Rigidbody>().isKinematic = true;
	}
}