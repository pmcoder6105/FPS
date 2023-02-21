//Stripped version of my CharacterMotor upgraded script, included in the Modern First Person Shooter Kit asset available at the asset store,
//containing a fully-functional crouching system!
using UnityEngine;
using System.Collections;

public class CharacterMover : MonoBehaviour{
	public bool canControl = true;
	
	Vector3 inputMoveDirection = Vector3.zero;
	
	bool inputJump = false;

	[System.Serializable]
	public class CharacterMotorMovement{
		public float maxForwardSpeed = 2.0f;
		public float maxBackwardSpeed = 2.0f;
		public float maxRunSpeed = 3.5f;
		
		public AnimationCurve slopeSpeedMultiplier = new AnimationCurve(new Keyframe(-90, 1), new Keyframe(0, 1), new Keyframe(90, 0));
		
		public float maxGroundAcceleration = 30.0f;
		public float maxAirAcceleration = 20.0f;
		
		public float gravity = 10.0f;
		public float maxFallSpeed = 20.0f;
		
		[HideInInspector]
		public CollisionFlags collisionFlags; 

		[HideInInspector]
		public Vector3 velocity;
		
		[HideInInspector]
		public Vector3 frameVelocity = Vector3.zero;
		
		[HideInInspector]
		public Vector3 hitPoint = Vector3.zero;
		
		[HideInInspector]
		public Vector3 lastHitPoint = new Vector3(Mathf.Infinity, 0, 0);
	}
	public CharacterMotorMovement movement = new CharacterMotorMovement();
	
	public enum MovementTransferOnJump{
		None, // The jump is not affected by velocity of floor at all.
		InitTransfer, // Jump gets its initial velocity from the floor, then gradualy comes to a stop.
		PermaTransfer, // Jump gets its initial velocity from the floor, and keeps that velocity until landing.
		PermaLocked // Jump is relative to the movement of the last touched floor and will move together with that floor.
	}
	
	[System.Serializable]
	public class CharacterMotorJumping{
		public bool enabled = true;
		public float baseHeight = 1.0f;
		//var extraHeight : float = 4.1;
		public float perpAmount = 0.0f;
		public float steepPerpAmount = 0.5f;

		[HideInInspector]
		public bool jumping = false;

		[HideInInspector]
		public bool holdingJumpButton = false;
	
		[HideInInspector]
		public float lastStartTime = 0.0f;
	
		[HideInInspector]
		public float lastButtonDownTime = -100;
	
		[HideInInspector]
		public Vector3 jumpDir = Vector3.up;
	}
	public CharacterMotorJumping jumping = new CharacterMotorJumping();
	
	[System.Serializable]
	public class CharacterMotorMovingPlatform{
		public bool enabled = true;
		public MovementTransferOnJump movementTransfer = new MovementTransferOnJump();
		
		[HideInInspector]
		public Transform hitPlatform;
	
		[HideInInspector]
		public Transform activePlatform;
	
		[HideInInspector]
		public Vector3 activeLocalPoint;
	
		[HideInInspector]
		public Vector3 activeGlobalPoint;
	
		[HideInInspector]
		public Quaternion activeLocalRotation;
	
		[HideInInspector]
		public Quaternion activeGlobalRotation;
	
		[HideInInspector]
		public Matrix4x4 lastMatrix;
	
		[HideInInspector]
		public Vector3 platformVelocity;
	
		[HideInInspector]
		public bool newPlatform;
	}
	public CharacterMotorMovingPlatform movingPlatform = new CharacterMotorMovingPlatform();
	
	[System.Serializable]
	public class CharacterMotorSliding{
		public bool enabled = true;
		
		public float slidingSpeed = 15;
		
		public float sidewaysControl = 1.0f;
		
		// How much can the player influence the sliding speed?
		// If the value is 0.5 the player can speed the sliding up to 150% or slow it down to 50%.
		public float speedControl = 0.4f;
	}
	public CharacterMotorSliding sliding = new CharacterMotorSliding();
	
	[HideInInspector]
	public bool grounded = true;

	[HideInInspector]
	public Vector3 groundNormal = Vector3.zero;

	[HideInInspector]
	public float fallDamage;

	[HideInInspector]
	public float normalWalkSpeed;
	
	private Vector3 lastGroundNormal = Vector3.zero;
	
	private CharacterController controller;
	
	void Awake(){
		controller = GetComponent<CharacterController>();
		normalWalkSpeed = movement.maxForwardSpeed;
	}

	private void UpdateFunction(){
		Vector3 velocity = movement.velocity;
		
		velocity = ApplyInputVelocityChange(velocity);
		
		velocity = ApplyGravityAndJumping(velocity);
		
		Vector3 moveDistance = Vector3.zero;
		if(MoveWithPlatform()){
			Vector3 newGlobalPoint = movingPlatform.activePlatform.TransformPoint(movingPlatform.activeLocalPoint);
			moveDistance = (newGlobalPoint - movingPlatform.activeGlobalPoint);
			if(moveDistance != Vector3.zero){
				controller.Move(moveDistance);
			}
			
			Quaternion newGlobalRotation = movingPlatform.activePlatform.rotation * movingPlatform.activeLocalRotation;
			Quaternion rotationDiff = newGlobalRotation * Quaternion.Inverse(movingPlatform.activeGlobalRotation);
			
			var yRotation = rotationDiff.eulerAngles.y;
			if(yRotation != 0){
				transform.Rotate(0, yRotation, 0);
			}
		}
		
		Vector3 lastPosition = transform.position;
		
		Vector3 currentMovementOffset = velocity * Time.deltaTime;
		
		float pushDownOffset = Mathf.Max(controller.stepOffset, new Vector3(currentMovementOffset.x, 0, currentMovementOffset.z).magnitude);
		if(grounded)
			currentMovementOffset -= pushDownOffset * Vector3.up;
		
		movingPlatform.hitPlatform = null;
		groundNormal = Vector3.zero;
		
		movement.collisionFlags = controller.Move (currentMovementOffset);
		
		movement.lastHitPoint = movement.hitPoint;
		lastGroundNormal = groundNormal;
		
		if(movingPlatform.enabled && movingPlatform.activePlatform != movingPlatform.hitPlatform){
			if(movingPlatform.hitPlatform != null){
				movingPlatform.activePlatform = movingPlatform.hitPlatform;
				movingPlatform.lastMatrix = movingPlatform.hitPlatform.localToWorldMatrix;
				movingPlatform.newPlatform = true;
			}
		}
		
		Vector3 oldHVelocity = new Vector3(velocity.x, 0, velocity.z);
		movement.velocity = (transform.position - lastPosition) / Time.deltaTime;
		Vector3 newHVelocity = new Vector3(movement.velocity.x, 0, movement.velocity.z);
		
		if(oldHVelocity == Vector3.zero){
			movement.velocity = new Vector3(0, movement.velocity.y, 0);
		}
		else{
			float projectedNewVelocity = Vector3.Dot(newHVelocity, oldHVelocity) / oldHVelocity.sqrMagnitude;
			movement.velocity = oldHVelocity * Mathf.Clamp01(projectedNewVelocity) + movement.velocity.y * Vector3.up;
		}
		
		if(movement.velocity.y < velocity.y - 0.001){
			if(movement.velocity.y < 0)
				movement.velocity.y = velocity.y;
			else
				jumping.holdingJumpButton = false;
		}
		
		if(grounded && !IsGroundedTest()){
			grounded = false;
			
			if(movingPlatform.enabled && (movingPlatform.movementTransfer == MovementTransferOnJump.InitTransfer || movingPlatform.movementTransfer == MovementTransferOnJump.PermaTransfer)){
				movement.frameVelocity = movingPlatform.platformVelocity;
				movement.velocity += movingPlatform.platformVelocity;
			}
			
			//SendMessage("OnFall", SendMessageOptions.DontRequireReceiver);
			transform.position += pushDownOffset * Vector3.up;
		}
		else if(!grounded && IsGroundedTest()){
			grounded = true;
			jumping.jumping = false;
			StartCoroutine(SubtractNewPlatformVelocity());
			
			//SendMessage("OnLand", SendMessageOptions.DontRequireReceiver);
			if(fallDamage > 30)
				SendMessageUpwards("ApplyFallDamage", fallDamage - 30, SendMessageOptions.DontRequireReceiver);
			fallDamage = 0;
		}
		if(MoveWithPlatform()){
			movingPlatform.activeGlobalPoint = transform.position + Vector3.up * (controller.center.y - controller.height * 0.5f + controller.radius);
			movingPlatform.activeLocalPoint = movingPlatform.activePlatform.InverseTransformPoint(movingPlatform.activeGlobalPoint);
			
			movingPlatform.activeGlobalRotation = transform.rotation;
			movingPlatform.activeLocalRotation = Quaternion.Inverse(movingPlatform.activePlatform.rotation) * movingPlatform.activeGlobalRotation; 
		}
	}
	
	void FixedUpdate(){
		UpdateFunction();
		if(!grounded){
			fallDamage -= Time.fixedDeltaTime * controller.velocity.y * 10;
		}
		Vector3 conCenter = controller.center;
		if(controller.center.y != controller.height / 2.0f){
			conCenter.y = controller.height / 2.0f;
		}
		controller.center = conCenter;
		if(controller.height < 1.0f || controller.height > 1.8f){
			controller.height = Mathf.Clamp(controller.height, 1.0f, 1.8f);
		}

		if(movingPlatform.enabled){
			if(movingPlatform.activePlatform != null){
				if(!movingPlatform.newPlatform){
					movingPlatform.platformVelocity = (movingPlatform.activePlatform.localToWorldMatrix.MultiplyPoint3x4(movingPlatform.activeLocalPoint) - movingPlatform.lastMatrix.MultiplyPoint3x4(movingPlatform.activeLocalPoint)) / Time.deltaTime;
				}
				movingPlatform.lastMatrix = movingPlatform.activePlatform.localToWorldMatrix;
				movingPlatform.newPlatform = false;
			}
			else{
				movingPlatform.platformVelocity = Vector3.zero;	
			}
		}
	}
	
	void Update(){
		if(Input.GetButtonDown("Run")){
			movement.maxForwardSpeed = movement.maxRunSpeed;
		}
		else if(Input.GetButtonUp("Run")){
			movement.maxForwardSpeed = normalWalkSpeed;
		}
		Vector3 directionVector = Vector3.zero;
		directionVector = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
		if(directionVector != Vector3.zero){
			var directionLength = directionVector.magnitude;
			directionVector = directionVector / directionLength;
			directionLength = Mathf.Min(1, directionLength);
			directionLength = directionLength * directionLength;
			directionVector = directionVector * directionLength;
		}
		inputMoveDirection = transform.rotation * directionVector;
		inputJump = Input.GetButton("Jump");
	}
	
	private Vector3 ApplyInputVelocityChange(Vector3 velocity){	
		if(!canControl)
			inputMoveDirection = Vector3.zero;
		
		Vector3 desiredVelocity;
		if(grounded && TooSteep()){
			desiredVelocity = new Vector3(groundNormal.x, 0, groundNormal.z).normalized;
			var projectedMoveDir = Vector3.Project(inputMoveDirection, desiredVelocity);
			desiredVelocity = desiredVelocity + projectedMoveDir * sliding.speedControl + (inputMoveDirection - projectedMoveDir) * sliding.sidewaysControl;
			desiredVelocity *= sliding.slidingSpeed;
		}
		else
			desiredVelocity = GetDesiredHorizontalVelocity();
		
		if(movingPlatform.enabled && movingPlatform.movementTransfer == MovementTransferOnJump.PermaTransfer){
			desiredVelocity += movement.frameVelocity;
			desiredVelocity.y = 0;
		}
		
		if(grounded)
			desiredVelocity = AdjustGroundVelocityToNormal(desiredVelocity, groundNormal);
		else
			velocity.y = 0;
		
		// Enforce max velocity change
		float maxVelocityChange = GetMaxAcceleration(grounded) * Time.deltaTime;
		Vector3 velocityChangeVector = (desiredVelocity - velocity);
		if(velocityChangeVector.sqrMagnitude > maxVelocityChange * maxVelocityChange){
			velocityChangeVector = velocityChangeVector.normalized * maxVelocityChange;
		}
		if(grounded || canControl)
			velocity += velocityChangeVector;
		
		if(grounded){
			velocity.y = Mathf.Min(velocity.y, 0);
		}
		return velocity;
	}
	
	private Vector3 ApplyGravityAndJumping(Vector3 velocity){
		if(!inputJump || !canControl){
			jumping.holdingJumpButton = false;
			jumping.lastButtonDownTime = -100;
		}
		
		if(inputJump && jumping.lastButtonDownTime < 0 && canControl)
			jumping.lastButtonDownTime = Time.time;
		
		if(grounded)
			velocity.y = Mathf.Min(0, velocity.y) - movement.gravity * Time.deltaTime;
		else{
			velocity.y = movement.velocity.y - movement.gravity * Time.deltaTime;
			/*if(jumping.jumping && jumping.holdingJumpButton){
			if(Time.time < jumping.lastStartTime + jumping.extraHeight / CalculateJumpVerticalSpeed(jumping.baseHeight)){
				velocity += jumping.jumpDir * movement.gravity * Time.deltaTime;
			}
		}*/
			velocity.y = Mathf.Max(velocity.y, -movement.maxFallSpeed);
		}
		
		if(grounded){
			if(jumping.enabled && canControl && (Time.time - jumping.lastButtonDownTime < 0.2)){
				grounded = false;
				jumping.jumping = true;
				jumping.lastStartTime = Time.time;
				jumping.lastButtonDownTime = -100;
				jumping.holdingJumpButton = true;
				
				if(TooSteep())
					jumping.jumpDir = Vector3.Slerp(Vector3.up, groundNormal, jumping.steepPerpAmount);
				else
					jumping.jumpDir = Vector3.Slerp(Vector3.up, groundNormal, jumping.perpAmount);
				
				velocity.y = 0;
				velocity += jumping.jumpDir * CalculateJumpVerticalSpeed(jumping.baseHeight);
				
				if(movingPlatform.enabled && (movingPlatform.movementTransfer == MovementTransferOnJump.InitTransfer || movingPlatform.movementTransfer == MovementTransferOnJump.PermaTransfer)){
					movement.frameVelocity = movingPlatform.platformVelocity;
					velocity += movingPlatform.platformVelocity;
				}
			}
			else{
				jumping.holdingJumpButton = false;
			}
		}
		
		return velocity;
	}
	
	void OnControllerColliderHit(ControllerColliderHit hit){
		if(hit.normal.y > 0 && hit.normal.y > groundNormal.y && hit.moveDirection.y < 0){
			if((hit.point - movement.lastHitPoint).sqrMagnitude > 0.001 || lastGroundNormal == Vector3.zero)
				groundNormal = hit.normal;
			else
				groundNormal = lastGroundNormal;
			
			movingPlatform.hitPlatform = hit.collider.transform;
			movement.hitPoint = hit.point;
			movement.frameVelocity = Vector3.zero;
		}
	}
	
	private IEnumerator SubtractNewPlatformVelocity(){
		if(movingPlatform.enabled && (movingPlatform.movementTransfer == MovementTransferOnJump.InitTransfer || movingPlatform.movementTransfer == MovementTransferOnJump.PermaTransfer)){
			if(movingPlatform.newPlatform){
				Transform platform = movingPlatform.activePlatform;
				yield return new WaitForFixedUpdate();
				yield return new WaitForFixedUpdate();
				if(grounded && platform == movingPlatform.activePlatform)
					yield return 1;
			}
			movement.velocity -= movingPlatform.platformVelocity;
		}
	}
	
	private bool MoveWithPlatform(){
		return(
			movingPlatform.enabled
			&& (grounded || movingPlatform.movementTransfer == MovementTransferOnJump.PermaLocked)
			&& movingPlatform.activePlatform != null);
	}
	
	private Vector3 GetDesiredHorizontalVelocity(){
		Vector3 desiredLocalDirection = transform.InverseTransformDirection(inputMoveDirection);
		float maxSpeed = MaxSpeedInDirection(desiredLocalDirection);
		if(grounded){
			var movementSlopeAngle = Mathf.Asin(movement.velocity.normalized.y) * Mathf.Rad2Deg;
			maxSpeed *= movement.slopeSpeedMultiplier.Evaluate(movementSlopeAngle);
		}
		return transform.TransformDirection(desiredLocalDirection * maxSpeed);
	}
	
	private Vector3 AdjustGroundVelocityToNormal(Vector3 hVelocity, Vector3 groundNormal){
		Vector3 sideways = Vector3.Cross(Vector3.up, hVelocity);
		return Vector3.Cross(sideways, groundNormal).normalized * hVelocity.magnitude;
	}
	
	private bool IsGroundedTest(){
		return(groundNormal.y > 0.01);
	}
	
	float GetMaxAcceleration(bool grounded){
		if(grounded)
			return movement.maxGroundAcceleration;
		else
			return movement.maxAirAcceleration;
	}
	
	float CalculateJumpVerticalSpeed(float targetJumpHeight){
		return Mathf.Sqrt(2 * targetJumpHeight * movement.gravity);
	}
	
	bool IsJumping(){
		return jumping.jumping;
	}
	
	bool IsSliding(){
		return(grounded && sliding.enabled && TooSteep());
	}
	
	bool IsTouchingCeiling(){
		return(movement.collisionFlags & CollisionFlags.CollidedAbove) != 0;
	}
	
	bool IsGrounded(){
		return grounded;
	}
	
	bool TooSteep(){
		return(groundNormal.y <= Mathf.Cos(controller.slopeLimit * Mathf.Deg2Rad));
	}
	
	Vector3 GetDirection(){
		return inputMoveDirection;
	}
	
	float MaxSpeedInDirection(Vector3 desiredMovementDirection){
		if(desiredMovementDirection == Vector3.zero)
			return 0;
		else{
			float zAxisEllipseMultiplier = (desiredMovementDirection.z > 0 ? movement.maxForwardSpeed : movement.maxBackwardSpeed) / (movement.maxForwardSpeed);
			Vector3 temp = new Vector3(desiredMovementDirection.x, 0, desiredMovementDirection.z / zAxisEllipseMultiplier).normalized;
			float length = new Vector3(temp.x, 0, temp.z * zAxisEllipseMultiplier).magnitude * (movement.maxForwardSpeed);
			return length;
		}
	}
	
	void SetVelocity(Vector3 velocity){
		grounded = false;
		movement.velocity = velocity;
		movement.frameVelocity = Vector3.zero;
	}
}