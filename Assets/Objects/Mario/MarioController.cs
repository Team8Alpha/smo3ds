using UnityEngine;
using System.Collections;
using System;

public enum plState
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
	[HideInInspector] public static plState myState;
	[HideInInspector] public int mySubState = 0;

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
	[HideInInspector] public bool isRotate			= true;

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
	[HideInInspector] public string anim_land		= "landShort";

	[HideInInspector] public scrBehaviorCappy		cappy;
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
	[HideInInspector] private bool isMovingAir 		= false; //if falling direction is active
	[HideInInspector] private Vector3 directionAir; //used for falling direction
	[HideInInspector] private Vector3 lastPosition;
	[HideInInspector] public float tsldSpeed = 1; //treshold speed, like 0.1th of the speed (in this case just 1)

	[HideInInspector] CapsuleCollider capsColl1;
	[HideInInspector] CapsuleCollider capsColl2;
	private float[,] jumpForces = new float[3, 3] {
		{ 7.94f, 1.05f, 2.58f }, // Jump 1
		{ 8.73f, 1.365f, 3.12f }, // Jump 2
		{ 11.57f, 3.75f, 5.5f } // Jump 3
	};
	//jump force max,  jump height min,  jump height max

    void Awake()
	{
		scr_main._f.SetCMD ("");

		// Disable AudioListener for the global values
		scr_main._f.GetComponent<AudioListener>().enabled = false;

		// Enable AudioListener for Mario's object
		GetComponent<AudioListener>().enabled = true;

		// Store references to Animator and Rigidbody components
		anim = GetComponent<Animator>();
		rb = GetComponent<Rigidbody>();
		capsColl1 = GetComponents<CapsuleCollider>()[0];
		capsColl2 = GetComponents<CapsuleCollider>()[1];
        marioObject = this;
		ResetAnim ();

		groundedPosition = transform.position.y;
		lastPosition = transform.position;
	}

	void Update()
	{
		if (scr_main._f.isFocused) {

			HandleInput ();
			HandleMove ();
			HandleHack ();

			// Update Mario's animation and movement based on states
			switch (myState) {
			case plState.Ground: // Standing still, wait

				if (jumpAfterTimer > 0) {
					if (jumpType > 2) jumpAfterTimer = 0;
					if (jumpAfterTimer > 9) {//maximal
						jumpAfterTimer = 0;
						jumpType = 0;
					}
					jumpAfterTimer++;
				}
				if (transform.position.y < groundedPosition - 3)
					SetState (plState.Falling);
				break;

			case plState.Jumping: // Jumping from land normal
				float jumpedHeight = transform.position.y - lastGroundedPosition;

				if (jumpedHeight > 4.5f) { //JUMPING HIGH CAM
					groundedPosition = transform.position.y;
					MarioCam.marioCamera.confSmoothTime = 0.13f;
					MarioCam.marioCamera.confYOffset = 1;
				}

				if ((jumpedHeight > jumpForces[jumpType-1, 1] && !key_jump)
					|| (jumpedHeight > jumpForces[jumpType-1, 2] && key_jump)
					|| hasTouchedCeiling) {

					rb.AddForce (Vector3.down * jumpForces [jumpType-1, 0] /2.5f, ForceMode.VelocityChange);
					if (hasTouchedCeiling) {
						rb.velocity = new Vector3 (rb.velocity.x, 0, rb.velocity.z); 
						jumpAfterTimer = 0;
					}
					SetState (plState.Falling, 1); //substate 1, jumpfall
					
				}
				//once left ground, he jumped.
				if (!isGrounded)
					hasJumped = true;
				//if clipped up without even starting the jumpfall, he landed.
				if (isGrounded && hasJumped)
					SetState (plState.Landing);
				
				/*switch (mySubState) {
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
						rb.AddForce (Vector3.down * ((1 - (jumpedHeight / (maxJump / 0.1f))) * jumpForce - (jumpType/2)) * 100 * Time.deltaTime, ForceMode.Impulse);

						if (hasTouchedCeiling)
							jumpAfterTimer = 0;
						SetState (plState.Falling, 1); //substate 1, jumpfall
					}
					//once left ground, he jumped.
					if (!isGrounded)
						hasJumped = true;
					//if clipped up without even starting the jumpfall, he landed.
					if (isGrounded && hasJumped)
						SetState (plState.Landing);
					
					break;
				}*/
				break;
			case plState.Falling: //FALLING CAM
				switch (mySubState) {
				case 0: //camera follow
					groundedPosition = transform.position.y; 
					if (transform.position.y < lastGroundedPosition - 4) {
						if (transform.position.y > lastGroundedPosition - 5) {
							MarioCam.marioCamera.confSmoothTime = 10f;

						}
					}
					break;
				case 1://jump fall
					if (transform.position.y < lastGroundedPosition - 3)
						SetState (myState, 0);
					break;
				}

				if (isGrounded) {
					SetState (plState.Landing);
					MarioCam.marioCamera.ResetValue ();
				}
				break;

			case plState.Landing: //land
				SetState (plState.Ground);
				break;

			case plState.CaptureFly: //flying to capture enemy
				// Calculate the current percentage of the journey completed
				float distanceCovered = (Time.time - hackFlyStartTime) * hackFlyLength / 1;
				float journeyFraction = distanceCovered / hackFlyLength;

					// Calculate the current position along the Bezier curve
					Vector3 targetPosition = Vector3.zero;//Bezier (transform.position, cappy.capturedObject.transform.position + Vector3.up * 3 + (transform.position - cappy.capturedObject.transform.position).normalized * 1, cappy.capturedObject.transform.position, journeyFraction);

				// Calculate the additional movement vector based on the target position
				moveAdditional = targetPosition - transform.position;

				// Check if the movement is completed
				if (Time.time - hackFlyStartTime >= 1) {
						// Ensure the object ends up at the final position
						//CAP! transform.position = cappy.capturedObject.transform.position;
						SetState(plState.Ground);
					if (!isBlockBlocked)
						isBlocked = false;
					isCapturing = false;
					hasCaptured = true;
						//CAP! cappy.capturedObject.SendMessage ("setState", 6);
						for (int i = 0; i <= 8; i++)
						transform.GetChild (i).gameObject.SetActive (false);
					rb.useGravity = true;
					moveAdditional = Vector3.zero;
				}
				break;
			case plState.Squat:
				if (moveAdditional != Vector3.zero) {
					moveAdditional *= 0.8f;
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

		lastPosition = transform.position;
		wasMoving = isMoving;
		// Calculate the movement vector based on the input and current speed
		Vector3 movementVector = Vector3.forward * currentMoveSpeed * tsldSpeed * Time.deltaTime;
        
        // Move the character using the Rigidbody
		rb.MovePosition ((rb.position + (transform.rotation * Vector3.forward) * movementVector.magnitude) + Vector3.up * jumpVelocity + moveAdditional);
    }

    void HandleInput(){
        
		// Check if Mario is blocked or pressing L to move the camera
		if (isBlocked) {
			h = 0;
			v = 0;
			isMoving = false;

		} else {
			
			#if UNITY_EDITOR
				h = Input.GetAxisRaw ("Horizontal");
				v = Input.GetAxisRaw ("Vertical");

				key_jump = Input.GetKey (KeyCode.Space);
				key_backL = Input.GetKey (KeyCode.LeftControl);
				key_backR = Input.GetKey (KeyCode.RightControl);
				key_cap = Input.GetKey (KeyCode.LeftAlt);
			#else
				key_backL = UnityEngine.N3DS.GamePad.GetButtonHold(N3dsButton.L);
				if(key_backL){
					h = 0; v = 0;
					return;
				}
				h = UnityEngine.N3DS.GamePad.CirclePad.x;
				v = UnityEngine.N3DS.GamePad.CirclePad.y;

				key_jump = UnityEngine.N3DS.GamePad.GetButtonHold(N3dsButton.A) || UnityEngine.N3DS.GamePad.GetButtonHold(N3dsButton.B);
				key_backR = UnityEngine.N3DS.GamePad.GetButtonHold(N3dsButton.R);
				key_cap = UnityEngine.N3DS.GamePad.GetButtonHold(N3dsButton.X) || UnityEngine.N3DS.GamePad.GetButtonHold(N3dsButton.Y);
			#endif

			isMoving = h != 0 || v != 0;
			switch (myState) {
			case plState.Ground:
				if (key_jump || isJumpingSoon) {
					if (!lockJump) {
						lockJump = true;
						isJumpingSoon = false;
						SetState (plState.Jumping);
					} else if (!key_jump)
						lockJump = false;
				} else if (lockJump)
				if (!key_jump)
					lockJump = false;

				if (key_backR)
					SetState (plState.Squat);
				break;

			case plState.Jumping:
				if (!key_jump)
					lockJump = false;
				break;

			case plState.Squat:
				if (!key_backR) {
					moveAdditional = Vector3.zero;
					SetState (plState.Ground);
				}
				break;
			}
		}
	}

	public void SetState(plState state, int subState = 0)
	{
		myState = state;
		mySubState = subState;
		switch (state) {
		case plState.Ground:
			
			//reset some data
			SetCollider (1.6f);
			ResetSpeed ();
			ResetAnim ();
			if (isMoving) {
				SetAnim (anim_run, 0.05f);
			}
			break;
                
		case plState.Jumping:
			jumpType++;
			jumpAfterTimer = 1;
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

			rb.AddForce (jumpForces[jumpType-1,0] * Vector3.up, ForceMode.Impulse);
			break;

		case plState.Falling:
			switch (subState) {
			case 0://falling, below lastgroundedposition
				lastGroundedPosition = groundedPosition;
				MarioCam.marioCamera.confSmoothTime = 0.2f;
				MarioCam.marioCamera.confYOffset = 1;
				MarioCam.marioCamera.confCamDistance = MarioCam.marioCamera.defCamDistance - 1;
				SetAnim ("falling", 0.1f);
				break;
			case 1://falling after jump, still above lastgroundedposition
				break;
			}
			break;

		case plState.Landing:
			SetAnim (anim_land);

			hasJumped = false;
			break;

		case plState.CaptureFly: 
			hackFlyStartTime = Time.time;
			SetAnim ("captureFly");
			rb.useGravity = false;
			break;

		case plState.Squat:
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
		if (!isGrounded) {
			if (!isMovingAir) {
				if (!isMoving)
					directionAir = Vector3.zero;
				directionAir = (lastPosition - transform.position);
				directionAir.y = 0;
				tsldSpeed = 0.1f;
				isRotate = false;
				isMovingAir = true;
			}
			rb.MovePosition (rb.position - directionAir);
		}

		if (isMoving) {
			if (isGrounded) {
				if (isFixWalk) {
					SetAnim (anim_run);
					isFixWalk = false;
				}
				if (!wasMoving) {
					if (currentMoveSpeed > 1)
						SetAnim (anim_run);
					else
						SetAnim (anim_runStart);
				}
			}

			if (isRotate) {
				float tmp_walkRotation = 0;
				if (transform.rotation.y < 179 && transform.rotation.y > -179) {
					walkRotation += tmp_walkRotation / 76;
					tmp_walkRotation = Mathf.Atan2 (h, v) * Mathf.Rad2Deg;
				} else {
					tmp_walkRotation = 0;
				}
				transform.rotation = Quaternion.Euler (transform.eulerAngles.x, tmp_walkRotation + walkRotation + MarioCam.marioCamera.gameObject.transform.eulerAngles.y, transform.eulerAngles.z);
			}

			if (currentMoveSpeed < moveSpeed) {
				currentMoveSpeed += 0.3f;
			}

			if (currentMoveSpeed > moveSpeed)
				currentMoveSpeed = moveSpeed;


		} else {
			if (isGrounded) {
				if (currentMoveSpeed > 0) {
					currentMoveSpeed -= 0.5f;
					if(animLast != "dashBrake" && animLast != anim_land) SetAnim ("dashBrake", 0.3f);
				} else {
					if(currentMoveSpeed < 0) currentMoveSpeed = 0;
					if (animLast != anim_stand && animLast != anim_land && myState == plState.Ground) {
						SetAnim (anim_stand, 0.05f);
					}
				}
			}
		}

	}

	void HandleHack(){
		if (cappy != null) {
			if (plsUnhack) {
				plsUnhack = false;
			} else {
				//CAP! if (cappy.currentState == 3) //CAP!
				if (key_cap) {
					cappy.SetState (capState.Throw); //CAP!
				}
			}
		}
		if (isHacking) {
			if (hasCaptured) {
				if (key_backR || plsUnhack) {
					transform.GetChild (2).gameObject.SetActive (false);//hair
					transform.GetChild (1).gameObject.SetActive (true);//cap
					SetState (plState.Ground);
					//CAP! cappy.capturedObject.SendMessage ("setState", 7); //CAP!
					//CAP! cappy.capturedObject.tag 	= "Untagged"; //CAP!
					//CAP! if (cappy.capturedObject.GetComponent<Collider> () != null)
					//CAP! cappy.capturedObject.GetComponent<Collider> ().enabled = true;
					transform.Translate (0, 0, -2);
					ResetSpeed ();
					plsUnhack 		= false;
					isBlocked 		= false;
					isBlockBlocked 	= false;
					isHacking 		= false;
					scr_main._f.capMountPoint = "missingno";
					//CAP! var Mustache = cappy.capturedObject.transform.GetChild (0); //CAP!
					//CAP! if (Mustache.name == "Mustache" || Mustache.name == "Mustache__HairMT")
					//CAP! Mustache.gameObject.SetActive (false); //if mustache, place it at index 0
				}
			} else {
				if (!isCapturing) {
					isCapturing = true;
					isBlocked = true;
					SetState (plState.CaptureFly);
				}
			}
		} else {
			if (hasCaptured == true) {
				hasCaptured = false;
				//CAP! cappy.SetState (2);
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
			if (collis.gameObject.layer != scr_main.lyr_def)
			if (collis.gameObject.layer == scr_main.lyr_enemy || collis.gameObject.layer == scr_main.lyr_obj)
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
			scr_main._f.SetCMD(e.Message);
		}
	}
	void OnSensorTopEnter(Collider col){
		hasTouchedCeiling = true;
	}
	void OnSensorBottomEnter(Collider col){
		if(col.gameObject.layer != 20){
			isGrounded = true;
			isMovingAir = false;
			tsldSpeed = 1f;
			isRotate = true;
			SetState (plState.Landing);
		}
	}
	void OnSensorBottomStay(Collider col){
		if(col.gameObject.layer != 20){ // 20 = WALL LAYER
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
	public void OnSensorLODEnter(Collider coll){
		SetLOD (coll, true);
	}
	public void OnSensorLODExit(Collider coll){
		SetLOD (coll, false);
	}
	void SetLOD(Collider coll, bool state){
		if(coll.gameObject.GetComponent<paramObj>() != null)
		if(coll.gameObject.GetComponent<paramObj>().isLOD) 
			coll.gameObject.transform.GetChild(1).gameObject.SetActive (state);
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
		anim_land		= "landShort";
	}

	//SET
	public void SetSpeed(int _maxJump, float _moveSpeed, float scaleCap = 1)
	{
		maxJump = _maxJump;
		moveSpeed = _moveSpeed;
		cappy.transform.localScale = new Vector3 (scaleCap, scaleCap, scaleCap);
	}

	public void SetAnim(string animName, float transitionTime = 0, float animSpeed = 1)
	{
		if (!isAnim (animName)) {
			if(transitionTime == 0) anim.Play(animName);
			else anim.CrossFade (animName, transitionTime);
			anim.speed 		= animSpeed;
			animLast 		= animName;
		}
	}
	public void SetHand(int side, bool state){
		transform.GetChild (4 + side).gameObject.SetActive (state);
	}
	public void SetAnimMove(string stand, string run, string runStart){
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
		/*try {
			return anim.GetCurrentAnimatorStateInfo (0).IsName (anmName);
		} catch (Exception e) {
			return anim.GetCurrentAnimatorStateInfo (0).IsName (anmName);
		}*/
		return animLast == anmName;
	}

	// Bezier function (same as before)
	Vector3 Bezier(Vector3 a, Vector3 b, Vector3 c, float t)
	{
		return Vector3.Lerp(Vector3.Lerp(a, b, t), Vector3.Lerp(b, c, t), t);
	}
}
