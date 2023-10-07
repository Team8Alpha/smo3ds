using UnityEngine;
using System.Collections;
using System;

public enum MarioState
{
	Ground,
	Jumping,
	Falling,
	Landing,
	CaptureFly,
	Squat,
	GroundPound,
	WallJump
}

public class MarioController : MonoBehaviour
{
	public static MarioState myState;
	public int mySubState = 0;
	public int maxJump = 6;

	public float jumpForce = 2.5f;
	public float moveSpeed = 5.0f;

	[HideInInspector] public bool isGrounded 		= true;
	[HideInInspector] public bool isMoving 			= false;
	[HideInInspector] public bool wasMoving 		= false;
	[HideInInspector] public bool lockJump			= false;
	[HideInInspector] public bool isFixWalk 		= false; // fix run animation
	[HideInInspector] public bool hasJumped 		= false;
	[HideInInspector] public bool isJumpingSoon 	= false;

	[HideInInspector] public Animator anim;
	[HideInInspector] public Rigidbody rb;

	[HideInInspector] bool key_jump 				= false;
	[HideInInspector] bool key_backL 				= false;
	[HideInInspector] bool key_backR 				= false;
	[HideInInspector] bool key_cap 					= false;
    
	private float hackFlyLength 					= 0.5f;
	private float hackFlyStartTime 					= 0;

	private float h 								= 0;
	private float v 								= 0;
	private float currentMoveSpeed 					= 0;
	private float jumpVelocity 						= 0;

	[HideInInspector] public string anim_stand 		= "idle";
	[HideInInspector] public string anim_run 		= "run";
	[HideInInspector] public string anim_runStart 	= "runStart";

	[HideInInspector] public scr_behaviorMarioCap 	cappy;
	[HideInInspector] public static MarioController marioObject;

	[HideInInspector] public Vector3 moveAdditional = Vector3.zero;
	[HideInInspector] public float groundedPosition = 0; // latest floor position
	private float walkRotation 						= 0f; // stores the walk rotation offset or something
	[HideInInspector] public bool hasCaptured 		= false;
	private bool isCapturing 						= false;
	private RaycastHit hit;
	//private float fvar0 							= 0; // temporary var
	public bool isBlocked 							= false;
	[HideInInspector] public bool isHacking 		= false; // hack = modify/take control of object
	public bool isBlockBlocked 						= false; // to prevent it from setting block to false, if it handles multiple blocks...
	[HideInInspector] public bool plsUnhack 		= false;
	[HideInInspector] public string animLast 		= "idle";
	[HideInInspector] int jumpAfterTimer 			= 0; //timer till it refuses to execute double jump
	[HideInInspector] int jumpType 					= 0;
	[HideInInspector] bool hasTouchedCeiling 		= false;
	[HideInInspector] float lastGroundedPosition 	= 0;

	[HideInInspector] CapsuleCollider capsColl1;
	[HideInInspector] CapsuleCollider capsColl2;

    void Awake()
	{
		// Disable AudioListener for the global values
		scr_gameInit.globalValues.GetComponent<AudioListener>().enabled = false;

		// Enable AudioListener for Mario's object
		GetComponent<AudioListener>().enabled = true;

		// Store references to Animator and Rigidbody components
		anim = GetComponent<Animator>();
		rb = GetComponent<Rigidbody>();
		capsColl1 = GetComponents<CapsuleCollider>()[0];
		capsColl2 = GetComponents<CapsuleCollider>()[1];
        marioObject = this;

		groundedPosition = transform.position.y;
	}

	void Update()
	{
		if (scr_gameInit.globalValues.isFocused) {

			HandleInput ();

			if (!isBlocked)
				HandleMove ();

			HandleHack ();

			// Update Mario's animation and movement based on states
			switch (myState) {
			case MarioState.Ground: // Standing still, wait

				if (jumpAfterTimer > 0) {
					if (jumpType > 2) jumpAfterTimer = 0;
					if (jumpAfterTimer > 9) {//maximal
						jumpAfterTimer = 0;
						jumpType = 0;
					}
					jumpAfterTimer++;
				}
				if (transform.position.y < lastGroundedPosition-1)
					SetState (MarioState.Falling);
				break;

			case MarioState.Jumping: // Jumping from land normal
				float jumpedHeight = transform.position.y - lastGroundedPosition;

				if (jumpedHeight > 5) { //JUMPING HIGH CAM
					groundedPosition = transform.position.y;
					MarioCam.marioCamera.confSmoothTime = 0.14f;
					MarioCam.marioCamera.confYOffset = 1;
				}
				
				switch (mySubState) {
				case 0:
					rb.AddForce (Vector3.up * (jumpForce + (jumpType / 3)) * 100 * Time.deltaTime, ForceMode.Impulse); //start accelerating up
					mySubState++;
					jumpAfterTimer = 1;
					break;
				case 1:
					//keep velocity x z


					//force down when height reached
					float jumpingMax = (maxJump + (jumpType*2))/2f;
					if ((key_jump && jumpedHeight > jumpingMax && jumpType != 3 || hasTouchedCeiling)//if condition(reached top), stop accelerating and start falling
						|| (!key_jump && jumpedHeight > jumpingMax/2.4f && jumpType != 3)
						|| (jumpedHeight > jumpingMax && jumpType == 3)) { //TODO: more efficient...

						//force down
						rb.AddForce (Vector3.down * ((1 - (jumpedHeight / (maxJump / 0.1f))) * jumpForce - (jumpType/2)) * 130 * Time.deltaTime, ForceMode.VelocityChange);

						if (hasTouchedCeiling)
							jumpAfterTimer = 0;
						SetState (MarioState.Falling, 1); //substate 1, jumpfall
					}
					//once left ground, he jumped.
					if (!isGrounded)
						hasJumped = true;
					//if clipped up without even starting the jumpfall, he landed.
					if (isGrounded && hasJumped)
						SetState (MarioState.Landing);
					
					break;
				}
				break;
			case MarioState.Falling: //FALLING CAM
				switch (mySubState) {
				case 0: //camera follow
					groundedPosition = transform.position.y; 
					if (transform.position.y < lastGroundedPosition - 3 && transform.position.y > lastGroundedPosition - 4)
						MarioCam.marioCamera.confSmoothTime = 0.7f;
					break;
				case 1://jump fall
					if (transform.position.y <= lastGroundedPosition)
						SetState (myState, 0);
					break;
				}

				if (isGrounded) {
					SetState (MarioState.Landing);
					MarioCam.marioCamera.ResetValue ();
				}
				break;

			case MarioState.Landing: //land
				SetState (MarioState.Ground, 1);
				break;

			case MarioState.CaptureFly: //flying to capture enemy
				// Calculate the current percentage of the journey completed
				float distanceCovered = (Time.time - hackFlyStartTime) * hackFlyLength / 1;
				float journeyFraction = distanceCovered / hackFlyLength;

				// Calculate the current position along the Bezier curve
				Vector3 targetPosition = Bezier (transform.position, cappy.capturedObject.transform.position + Vector3.up * 3 + (transform.position - cappy.capturedObject.transform.position).normalized * 1, cappy.capturedObject.transform.position, journeyFraction);

				// Calculate the additional movement vector based on the target position
				moveAdditional = targetPosition - transform.position;

				// Check if the movement is completed
				if (Time.time - hackFlyStartTime >= 1) {
					// Ensure the object ends up at the final position
					transform.position = cappy.capturedObject.transform.position;
					SetState (MarioState.Ground);
					if (!isBlockBlocked)
						isBlocked = false;
					isCapturing = false;
					hasCaptured = true;
					cappy.capturedObject.SendMessage ("setState", 6);
					for (int i = 0; i <= 8; i++)
						transform.GetChild (i).gameObject.SetActive (false);
					rb.useGravity = true;
					moveAdditional = Vector3.zero;
				}
				break;
			case MarioState.Squat:
				if (moveAdditional != Vector3.zero) {
					moveAdditional *= 0.9f;
					if (moveAdditional.magnitude < 0.04f) {
						moveAdditional = Vector3.zero;
						isBlocked = false;
					}
				} else
					isBlocked = false;
				break;
			}
		}
		hasTouchedCeiling = false;

		wasMoving = isMoving;
		// Calculate the movement vector based on the input and current speed
		Vector3 movementVector = Vector3.forward * currentMoveSpeed * Time.deltaTime;
        
        // Move the character using the Rigidbody
		rb.MovePosition ((rb.position + (transform.rotation * Vector3.forward) * movementVector.magnitude) + Vector3.up * jumpVelocity + moveAdditional);
    }

    void HandleInput(){
        
#if UNITY_EDITOR
		h = Input.GetAxisRaw ("Horizontal");
		v = Input.GetAxisRaw ("Vertical");

		key_jump = Input.GetKey (KeyCode.Space);
		key_backL = Input.GetKey (KeyCode.LeftControl);
		key_backR = Input.GetKey (KeyCode.RightControl);
		key_cap = Input.GetKey (KeyCode.LeftShift);
#else
		h = UnityEngine.N3DS.GamePad.CirclePad.x;
		v = UnityEngine.N3DS.GamePad.CirclePad.y;

		key_jump = UnityEngine.N3DS.GamePad.GetButtonHold(N3dsButton.A) || UnityEngine.N3DS.GamePad.GetButtonHold(N3dsButton.B);
		key_backL = UnityEngine.N3DS.GamePad.GetButtonHold(N3dsButton.L);
		key_backR = UnityEngine.N3DS.GamePad.GetButtonHold(N3dsButton.R);
		key_cap = UnityEngine.N3DS.GamePad.GetButtonHold(N3dsButton.X) || UnityEngine.N3DS.GamePad.GetButtonHold(N3dsButton.Y);
#endif

		// Check if Mario is blocked
		if (isBlocked) {
			h = 0;
			v = 0;
			isMoving = false;
		} else {
			isMoving = h != 0 || v != 0;
			switch (myState) {
			case MarioState.Ground:
				if (key_jump || isJumpingSoon) {
					if (!lockJump) {
						lockJump = true;
						isJumpingSoon = false;
						SetState (MarioState.Jumping);
					} else if (!key_jump)
						lockJump = false;
				} else if (lockJump)
				if (!key_jump)
					lockJump = false;

				if (key_backL)
					SetState (MarioState.Squat);
				break;

			case MarioState.Jumping:
				if (!key_jump)
					lockJump = false;
				break;

			case MarioState.Squat:
				if (!key_backL) {
					moveAdditional = Vector3.zero;
					SetState (MarioState.Ground);
				}
				break;
			}
		}
	}

	public void SetState(MarioState state, int subState = 0)
	{
		myState = state;
		mySubState = subState;
		switch (state) {
		case MarioState.Ground:
			
			//reset some data
			SetCollider (1.6f);
			ResetSpeed ();
			ResetAnim ();

			switch (subState) {
			case 0:
				SetAnim (anim_stand);
				break;
			}
			if (isMoving)
				isFixWalk = true;
			break;
                
		case MarioState.Jumping:
			jumpType++;
			switch (jumpType) {
			case 2:
				SetAnim ("jump2");
				break;
			case 3:
				if (currentMoveSpeed < 6) {
					SetAnim ("jump");
					jumpType = 1;
					break;
				}
				SetAnim ("jump3");
				break;
			default:
				SetAnim ("jump");
				jumpType = 1;
				break;
			}
			lastGroundedPosition = groundedPosition;
			break;

		case MarioState.Falling:
			switch (subState) {
			case 0://falling, below lastgroundedposition
				lastGroundedPosition = groundedPosition;
				MarioCam.marioCamera.confSmoothTime = 0.2f;
				MarioCam.marioCamera.confYOffset = 0;
				MarioCam.marioCamera.confCamDistance = MarioCam.marioCamera.defCamDistance - 2;
				break;
			case 1://falling after jump, still above lastgroundedposition
				break;
			}
			break;

		case MarioState.Landing:
			SetAnim ("land", 0.1f, 1, false);

			hasJumped = false;
			break;

		case MarioState.CaptureFly: 
			hackFlyStartTime = Time.time;
			SetAnim ("captureFly");
			rb.useGravity = false;
			break;

		case MarioState.Squat:
			SetCollider (0.94f);
			SetSpeed (maxJump, 1.4f);
			if (isMoving)
				SetAnim ("squatStart_w", 0.1f);
			else
				SetAnim ("squatStart_s", 0.1f);
			isBlocked = true;

			moveAdditional = transform.rotation * Vector3.forward * (currentMoveSpeed / 50);
			break;
		}

	}


	void HandleMove(){
		GetComponent<Animator>().SetFloat("Speed", currentMoveSpeed);
		if (isMoving) {

			if ((!wasMoving || isFixWalk) && isGrounded) {
				isFixWalk = false;
			}

                float tmp_walkRotation = 0;
			if (transform.rotation.y < 179 && transform.rotation.y > -179) {

				walkRotation += tmp_walkRotation / 76;
				tmp_walkRotation = Mathf.Atan2 (h, v) * Mathf.Rad2Deg;
			} else {
				tmp_walkRotation = 0;
			}
			transform.rotation = Quaternion.Euler (transform.eulerAngles.x, tmp_walkRotation + walkRotation + MarioCam.marioCamera.gameObject.transform.eulerAngles.y, transform.eulerAngles.z);

			if (currentMoveSpeed < moveSpeed) {
				currentMoveSpeed += 0.5f;
			}

            if (currentMoveSpeed > moveSpeed)
                currentMoveSpeed = moveSpeed;

		} else {
			if (currentMoveSpeed > 0) currentMoveSpeed = 0;
            if (wasMoving && isGrounded)
                SetAnim(anim_stand);
		}

	}

	void HandleHack(){
		if (cappy != null) {
			if (plsUnhack) {
				plsUnhack = false;
			} else {
				if (cappy.currentState == 3)
				if (key_cap) {
					cappy.SetState (0); //cappy handles everything.
				}
			}
		}
		if (isHacking) {
			if (hasCaptured) {
				if (key_backL || key_backR || plsUnhack) {
					transform.GetChild (2).gameObject.SetActive (false);//hair
					transform.GetChild (1).gameObject.SetActive (true);//cap
					SetState (MarioState.Ground);
					cappy.capturedObject.SendMessage ("setState", 7);
					cappy.capturedObject.tag 	= "Untagged";
					if (cappy.capturedObject.GetComponent<Collider> () != null)
						cappy.capturedObject.GetComponent<Collider> ().enabled = true;
					transform.Translate (0, 0, -2);
					ResetSpeed ();
					plsUnhack 		= false;
					isBlocked 		= false;
					isBlockBlocked 	= false;
					isHacking 		= false;
					scr_gameInit.globalValues.capMountPoint = "missingno";
					var Mustache = cappy.capturedObject.transform.GetChild (0);
					if (Mustache.name == "Mustache" || Mustache.name == "Mustache__HairMT")
						Mustache.gameObject.SetActive (false); //if mustache, place it at index 0
				}
			} else {
				if (!isCapturing) {
					isCapturing = true;
					isBlocked = true;
					SetState (MarioState.CaptureFly);
				}
			}
		} else {
			if (hasCaptured == true) {
				hasCaptured = false;
				cappy.SetState (2);
				for (int i = 0; i <= 8; i++) {
					transform.GetChild (i).gameObject.SetActive (true);
				}
				transform.GetChild (2).gameObject.SetActive (true); // hair
				transform.GetChild (1).gameObject.SetActive (false); // cap
			}
		}
	}

	void OnTriggerEnter(Collider collis)
	{
		try
		{
			if (collis.gameObject.layer != scr_gameInit.lyr_def)
			if (collis.gameObject.layer == scr_gameInit.lyr_enemy || collis.gameObject.layer == scr_gameInit.lyr_obj)
			{
				if(collis.GetComponent<paramObj>() != null) if(!collis.GetComponent<paramObj>().isTouch) return;

				if(hasTouchedCeiling){
					collis.gameObject.SendMessage("OnTouch", 4);
					return;
				}
				if (transform.position.y < collis.GetComponent<paramObj>().bCenterY())
					collis.gameObject.SendMessage("OnTouch", 2);
				else if(!hasTouchedCeiling)
					collis.gameObject.SendMessage("OnTouch", 3);
			}
		}
		catch (Exception e)
		{
			Debug.Log(e.Message);
		}
	}
	void OnSensorTopEnter(Collider col){
		hasTouchedCeiling = true;
	}
	void OnSensorBottomStay(Collider col){
		if(col.gameObject.layer != 20){ // 20 = WALL LAYER
			isGrounded = true;
			groundedPosition = transform.position.y;
		}
	}
	void OnSensorBottomExit(Collider col){
		if (col.gameObject.layer != 20)
			isGrounded = false;
	}
	void OnSensorBelowStay(Collider col){
		if (rb.velocity.y < 0 && !isGrounded && key_jump && col.gameObject.layer != 20)
			isJumpingSoon = true;
	}

	//RESET
	public void ResetSpeed()
	{
		maxJump 		= 4;
		moveSpeed 		= 8f;
	}

	public void ResetAnim()
	{
		anim_stand 		= "idle";
		anim_run 		= "run";
		anim_runStart 	= "runStart";
	}

	//SET
	public void SetSpeed(int _maxJump, float _moveSpeed, float scaleCap = 1)
	{
		maxJump = _maxJump;
		moveSpeed = _moveSpeed;
		cappy.transform.localScale = new Vector3 (scaleCap, scaleCap, scaleCap);
	}

	public void SetAnim(string animName, float transitionTime = 0, float animSpeed = 1, bool isInstant = true)
	{
		if (isAnim (animName)) {
			if(isInstant) anim.Play(animName);
			else anim.CrossFade (animName, transitionTime);
			anim.speed 		= animSpeed;
			animLast 		= animName;
		}
	}
	public void SetHand(int side, bool state){
		transform.GetChild (4 + side).gameObject.SetActive (state);
	}
	public void SetAnim(string stand, string run, string runStart){
		anim_runStart = runStart; anim_run = run; anim_stand = stand;
	}
	public void SetCollider(float height, float radius = 0.4f){
		capsColl1.center = new Vector3 (0, height/2, 0);
		capsColl1.radius = radius;
		capsColl1.height = height;
		capsColl2.center = new Vector3 (0, height/2, 0);
		capsColl2.radius = radius;
		capsColl2.height = height;
	}

	//CHECK
	public bool isAnim(string anmName)
	{
		try {
			return !anim.GetCurrentAnimatorStateInfo (0).IsName (anmName) && !anim.GetCurrentAnimatorStateInfo (1).IsName (anmName);
		} catch (Exception e) {
			return !anim.GetCurrentAnimatorStateInfo (0).IsName (anmName);
		}
	}

	// Bezier function (same as before)
	Vector3 Bezier(Vector3 a, Vector3 b, Vector3 c, float t)
	{
		return Vector3.Lerp(Vector3.Lerp(a, b, t), Vector3.Lerp(b, c, t), t);
	}
}
