using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]

public class SimpleThirdPersonController : MonoBehaviour {

	private Animator anim;
	private Rigidbody rigid;
	private float Xinput;
	private float Yinput;
	
	void Start () {
	anim = GetComponent<Animator>();
	rigid = GetComponent<Rigidbody>();
	rigid.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
	}
	
	void Update () {
	//Update animator values every frame
	UpdateAnimatorValues();
	
	//Get Input Values
	Xinput = Input.GetAxis("Horizontal");
	Yinput = Input.GetAxis("Vertical");
	
	//Rotate the character along it's  y axis
	transform.Rotate(0,Xinput * 2.0f,0);
	}
	
	void UpdateAnimatorValues(){
	anim.SetFloat("Walk",Yinput);
	anim.SetBool("Run",Input.GetKey(KeyCode.LeftShift));
	}
}
