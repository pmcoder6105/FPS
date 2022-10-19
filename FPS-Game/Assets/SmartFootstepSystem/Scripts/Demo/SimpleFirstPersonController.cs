using UnityEngine;
using System.Collections;

[RequireComponent(typeof (Rigidbody))]
[RequireComponent(typeof (CapsuleCollider))]
[RequireComponent(typeof (SmartFootstepSystem))]
public class SimpleFirstPersonController : MonoBehaviour {

	private Rigidbody rigid;
	private float Xinput;
	private float Yinput;
	public float movementSpeed = 8.0f;
	public float runMultiplier = 2.0f;
	public float footstepRate = 0.5f;
	private float groundCheckDistance;
	private float normalizedmovementSpeed;
	private float normalizedfootstepRate;
	private float nextFootstep; 
	private bool isGrounded;
	
	void Start () {
	rigid = GetComponent<Rigidbody>();
	rigid.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
	groundCheckDistance = GetComponent<CapsuleCollider>().height / 2 + .25f;
	
	//caches the values making them non editable in runtime
	normalizedfootstepRate = footstepRate;
	normalizedmovementSpeed = movementSpeed;
	}
	
	
	void Update () {
	//Get Input Values
	Xinput = Input.GetAxis("Horizontal");
	Yinput = Input.GetAxis("Vertical");
	
	isGrounded = Grounded();
	
	if(Xinput != 0 || Yinput != 0 ){
	
		if(!Input.GetKey(KeyCode.LeftShift)){
			//Walking
			footstepRate = normalizedfootstepRate;
			movementSpeed = normalizedmovementSpeed;
		}
		
		else{
			//Running
			footstepRate = normalizedfootstepRate / runMultiplier;
			movementSpeed = normalizedmovementSpeed * runMultiplier;
		}
			
		if(isGrounded && Time.time > nextFootstep){
			nextFootstep = Time.time + footstepRate;
			GetComponent<SmartFootstepSystem>().Footstep();
		}
	}
	
	//Move the character
	transform.Translate(Xinput * (movementSpeed / 2),0,Yinput * movementSpeed);
	}
	
	public bool Grounded(){
	RaycastHit hit;
	bool gr = false;
	
	if (Physics.Raycast(transform.position + (Vector3.up * 0.1f), Vector3.down, out hit, groundCheckDistance)){
			if(hit.collider){
				gr =  true;
			}
			else{
				gr = false;
			}
		}
		
		return gr;
	}
}
