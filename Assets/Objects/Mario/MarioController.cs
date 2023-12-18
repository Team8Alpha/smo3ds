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
	public float moveSpeed = 8.06f;

	[HideInInspector] public bool isGrounded 		= true;
	[HideInInspector] public bool isMoving 			= false;
	[HideInInspector] public bool wasMoving 		= false;
	[HideInInspector] public bool lockJump			= false;
	[HideInInspector] public bool hasJumped 		= false;
	[HideInInspector] public bool isJumpingSoon		= false;
	[HideInInspector] public bool isInstTurn		= false;
	[HideInInspector] public bool isTurning			= false;
	[HideInInspector] public bool isFreezeFall		= false;

	[HideInInspector] public Animator anim;
	[HideInInspector] public Rigidbody rb;
	
	[HideInInspector] bool key_jump 				= false;
	[HideInInspector] bool key_backL 				= false;
	[HideInInspector] bool key_backR 				= false;
	[HideInInspector] public bool key_cap 			= false;
    
	private float hackFlyLength 					= 0.5f;
	private float hackFlyStartTime 					= 0;

	private float h 								= 0;
	private float v 								= 0;
	private float currentMoveSpeed 					= 0;
	private float jumpVelocity 						= 0;
	private float currentTurnSpeed					= 0;
	private float currentRotation					= 0;

	[HideInInspector] public string anim_stand 		= "idle";
	[HideInInspector] public string anim_run 		= "run";
	[HideInInspector] public string anim_runStart 	= "runStart";
	[HideInInspector] public string anim_land		= "landShort";

	[HideInInspector] public scrBehaviorCappy		cappy;
	[HideInInspector] public static MarioController marioObject;

	[HideInInspector] public Vector3 moveAdditional = Vector3.zero;
	[HideInInspector] public float groundedPosition = 0; // latest floor position
	[HideInInspector] public bool hasCaptured 		= false;
	private bool isCapturing 						= false;
	private RaycastHit hit;
	//private float fvar0 							= 0; // temporary var
	public bool isBlocked 							= false;
	[HideInInspector] public bool isHacking 		= false; // hack = modify/take control of object
	public bool isBlockBlocked 						= false; // to prevent it from setting block to false, if it handles multiple blocks...
	[HideInInspector] public bool plsUnhack 		= false;
	[HideInInspector] public string animLast 		= "idle";
	[HideInInspector] public int jumpType			= 0;
	[HideInInspector] int jumpAfterTimer 			= 0; //timer till it refuses to execute double jump
	[HideInInspector] bool hasTouchedCeiling 		= false;
	[HideInInspector] float lastGroundedPosition 	= 0;
	[HideInInspector] private bool isMovingAir 		= false; //if falling direction is active
	[HideInInspector] private float speedJumpH; //used for falling direction
	[HideInInspector] private Vector3 lastPosition;
	[HideInInspector] float timeStandTrns = 0.5f;

	[HideInInspector] CapsuleCollider capsColl1;
	[HideInInspector] CapsuleCollider capsColl2;
	//jump force max,  jump height min,  jump height max

	void Awake()
	{
		scr_main._f.GetComponent<AudioListener>().enabled = false;
		GetComponent<AudioListener>().enabled = true;

		// Store references to Animator and Rigidbody components
		anim = GetComponent<Animator>();
		rb = GetComponent<Rigidbody>();
		capsColl1 = GetComponents<CapsuleCollider>()[0];
		capsColl2 = GetComponents<CapsuleCollider>()[1];

		groundedPosition = transform.position.y;
		lastPosition = transform.position;

		marioObject = this;
	}

	void OnDestroy()
    {
		if(scr_main._f != null) scr_main._f.GetComponent<AudioListener>().enabled = true;
	}

	void Start()
    {
		SetState(plState.Ground);
    }

	void Update()
	{

		if (scr_main._f.isFocused)
		{
			HandleInput();
			HandleMove();
			HandleHack();

			// Update Mario's animation and movement based on states
			switch (myState)
			{
				case plState.Ground: // Standing still, wait

					if (jumpAfterTimer > 0)
					{
						if (jumpType > 2) jumpAfterTimer = 0;
						if (jumpAfterTimer > 9)
						{//maximal
							jumpAfterTimer = 0;
							jumpType = 0;
						}
						jumpAfterTimer++;
					}
					if (transform.position.y < groundedPosition - 3)
						SetState(plState.Falling);
					break;

				case plState.Jumping: // Jumping from land normal
					float jumpedHeight = transform.position.y - lastGroundedPosition;
					switch (mySubState)
					{
						case 0:

							if ((jumpedHeight > MarioTable.dataJump[jumpType - 1][1] && !key_jump)
								|| (jumpedHeight > MarioTable.dataJump[jumpType - 1][2] && key_jump)
								|| hasTouchedCeiling)
							{

								mySubState = 1;

							}
							//once left ground, he jumped.
							if (!isGrounded)
								hasJumped = true;
							//if clipped up without even starting the jumpfall, he landed.
							if (isGrounded && hasJumped)
								SetState(plState.Landing);
							break;
						case 1:
							rb.AddForce(Vector3.down * MarioTable.dataJump[jumpType - 1][0] * 5 * jumpedHeight, ForceMode.Acceleration);
							if (hasTouchedCeiling)
							{
								rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
								jumpAfterTimer = 0;
							}
							if (rb.velocity.y < 0)
								SetState(plState.Falling, 1); //substate 1, jumpfall
							break;
					}
					break;
				case plState.Falling: //FALLING CAM
					switch (mySubState)
					{
						case 0: //camera follow
							groundedPosition = transform.position.y;
							if (transform.position.y < lastGroundedPosition - 4)
							{
								if (transform.position.y > lastGroundedPosition - 5)
								{
									MarioCam.marioCamera.confSmoothTime = 10f;

								}
							}
							break;
						case 1://jump fall
							if (transform.position.y < lastGroundedPosition - 3)
								SetState(myState, 0);
							break;
					}

					if (isGrounded)
					{
						SetState(plState.Landing);
						MarioCam.marioCamera.ResetValue();
					}
					break;

				case plState.Landing: //land
					SetState(plState.Ground);
					break;

				case plState.CaptureFly: //flying to capture enemy
					// Calculate the current percentage of the journey completed
					float distanceCovered = (Time.time - hackFlyStartTime) * hackFlyLength / 1;
					float journeyFraction = distanceCovered / hackFlyLength;

					// Calculate the current position along the Bezier curve
					Vector3 posHackObj = cappy.hackedObj.transform.position;
					Vector3 targetPosition = Bezier (transform.position, posHackObj + Vector3.up * 3 + (transform.position - posHackObj).normalized * 1, posHackObj, journeyFraction);

					// Calculate the additional movement vector based on the target position
					moveAdditional = targetPosition - transform.position;

					// Check if the movement is completed
					if (Time.time - hackFlyStartTime >= 1)
					{
						// Ensure the object ends up at the final position
						transform.position = posHackObj;
						SetState(plState.Ground);
						if (!isBlockBlocked)
							isBlocked = false;
						isCapturing = false;
						hasCaptured = true;
						cappy.hackedObj.SendMessage ("setState", 6);
						SetVisible(false);
						rb.useGravity = true;
						moveAdditional = Vector3.zero;
					}
					break;
				case plState.Squat:
					if (moveAdditional != Vector3.zero)
					{
						moveAdditional *= 0.8f;
						if (moveAdditional.magnitude < 0.04f)
						{
							moveAdditional = Vector3.zero;
							isBlocked = false;
						}
					}
					else
						isBlocked = false;
					break;
			}
		}
		hasTouchedCeiling = false;

		lastPosition = transform.position;
		wasMoving = isMoving;

		float yVel = rb.velocity.y; //store old yvel
		Vector3 movementVector = transform.rotation * Vector3.forward * currentMoveSpeed + moveAdditional;// * Time.deltaTime; //mashed together movement math
		movementVector.y = isFreezeFall ? 0 : yVel; //reassign old yvel

		// Move the character using the Rigidbody
		rb.velocity = movementVector;
		rb.AddForce(Vector3.up * jumpVelocity);
	}

	void HandleInput()
	{

		// Check if Mario is blocked or pressing L to move the camera
		if (isBlocked)
		{
			h = 0;
			v = 0;
			isMoving = false;
		}
		else
		{

#if UNITY_EDITOR
			h = Input.GetAxisRaw("Horizontal");
			v = Input.GetAxisRaw("Vertical");

			key_jump = Input.GetKey(KeyCode.Space);
			key_backL = Input.GetKey(KeyCode.Q);
			key_backR = Input.GetKey(KeyCode.E);
			key_cap = Input.GetKey(KeyCode.X);
#else
				key_backL = UnityEngine.N3DS.GamePad.GetButtonHold(N3dsButton.L);

				h = UnityEngine.N3DS.GamePad.CirclePad.x;
				v = UnityEngine.N3DS.GamePad.CirclePad.y;

				key_jump = UnityEngine.N3DS.GamePad.GetButtonHold(N3dsButton.A) || UnityEngine.N3DS.GamePad.GetButtonHold(N3dsButton.B);
				key_backR = UnityEngine.N3DS.GamePad.GetButtonHold(N3dsButton.R);
				key_cap = UnityEngine.N3DS.GamePad.GetButtonHold(N3dsButton.X) || UnityEngine.N3DS.GamePad.GetButtonHold(N3dsButton.Y);
#endif
			if (key_backL)
			{
				h = 0; v = 0;
				return;
			}
			isMoving = h != 0 || v != 0;
			switch (myState)
			{
				case plState.Ground:
					if (key_jump || isJumpingSoon)
					{
						if (!lockJump)
						{
							lockJump = true;
							isJumpingSoon = false;
							SetState(plState.Jumping);
						}
						else if (!key_jump)
							lockJump = false;
					}
					else if (lockJump)
						if (!key_jump)
							lockJump = false;

					if (key_backR)
						SetState(plState.Squat);
					break;

				case plState.Jumping:
					if (!key_jump)
						lockJump = false;
					break;

				case plState.Squat:
					if (!key_backR)
					{
						moveAdditional = Vector3.zero;
						SetState(plState.Ground);
					}
					break;
			}
		}
	}

	public void SetState(plState state, int subState = 0)
	{
		myState = state;
		mySubState = subState;
		switch (state)
		{
			case plState.Ground:

				//reset some data
				SetCollider(1.6f);
				ResetSpeed();
				ResetAnim();
				MarioCam.marioCamera.ResetValue();

				currentTurnSpeed = MarioTable.speedTurnWalk;
				isInstTurn = false;
				break;

			case plState.Jumping:
				jumpType++;
				jumpAfterTimer = 1;
				float timeTrnsJump = 0.05f;
				switch (jumpType)
				{
					case 2:
						SetAnim("jump2", timeTrnsJump);
						break;
					case 3:
						if (currentMoveSpeed < 6)
						{
							SetAnim("jump", timeTrnsJump);
							jumpType = 1;
							break;
						}
						SetAnim("jump3", timeTrnsJump);
						break;
					default:
						SetAnim("jump", timeTrnsJump);
						jumpType = 1;
						break;
				}
				lastGroundedPosition = groundedPosition;

				currentTurnSpeed = MarioTable.speedTurnJump;
				isInstTurn = true;

				rb.AddForce(MarioTable.dataJump[jumpType - 1][0] * Vector3.up, ForceMode.Impulse);
				break;

			case plState.Falling:
				switch (subState)
				{
					case 0://falling, below lastgroundedposition
						lastGroundedPosition = groundedPosition;
						MarioCam.marioCamera.confSmoothTime = 0.2f;
						MarioCam.marioCamera.confYOffset = 1;
						MarioCam.marioCamera.confCamDistance = MarioCam.marioCamera.defCamDistance - 1;
						SetAnim("falling", 0.1f);
						break;
					case 1://falling after jump, still above lastgroundedposition
						break;
				}

				currentTurnSpeed = MarioTable.speedTurnFall;
				isInstTurn = true;

				break;

			case plState.Landing: //TODO: FALLING-LANDING HEIGHT STUFF
				//SetAnim(anim_land, 0.02f); // WIP LANDING HEIGHTS

				hasJumped = false;
				break;

			case plState.CaptureFly:
				hackFlyStartTime = Time.time;
				SetAnim("captureFly");
				rb.useGravity = false;
				break;

			case plState.Squat:
				SetCollider(0.94f);
				if (isMoving)
					SetAnim("squatStart_w", 0.1f);
				else
					SetAnim("squatStart_s", 0.1f);
				isBlocked = true;

				moveAdditional = transform.rotation * Vector3.forward * (currentMoveSpeed / 50);

				currentTurnSpeed = MarioTable.speedTurnSquat;
				isInstTurn = true;

				break;
		}

	}


	void HandleMove()
	{
		moveAdditional = Vector3.zero;
		if (!isGrounded)
		{
			if (!isMovingAir && myState == plState.Jumping)
			{
				if (!isMoving) speedJumpH = 0;
				else
				{
					speedJumpH = (Vector3.Distance(lastPosition, transform.position)) /2;
				}
				isMovingAir = true;
			}
			moveAdditional += transform.forward * speedJumpH;
		}

		if (isMoving || isMovingAir)
		{
			if (isGrounded)
			{
				if (!wasMoving)
				{
					if (currentMoveSpeed > 1)
						SetAnim(anim_run);
					else
						SetAnim(anim_runStart);
				}
			}

			{
				if (/*transform.rotation.y < 179 && transform.rotation.y > -179 && */!isTurning)
				{
					if(isMoving) currentRotation = Mathf.Atan2(h, v) * Mathf.Rad2Deg
						+ Mathf.Atan2(MarioCam.marioCamera.transform.forward.x, MarioCam.marioCamera.transform.forward.z) * Mathf.Rad2Deg;
				}

				if (isInstTurn)
				{
					float angleDistance = Mathf.Abs(Mathf.DeltaAngle(transform.eulerAngles.y, currentRotation));
					if (isTurning)
					{
						if (angleDistance > 100) currentTurnSpeed = Mathf.Abs(currentTurnSpeed) * 2;
						else if (angleDistance < 1) isTurning = false;
					}
					else
					{
						if (angleDistance > 100) isTurning = true;
					}
				}

				transform.rotation = Quaternion.Euler(transform.eulerAngles.x,
					Mathf.MoveTowardsAngle(transform.eulerAngles.y, currentRotation, currentTurnSpeed), 
					transform.eulerAngles.z);
			}

			if (!isMovingAir)
			{
				if (currentMoveSpeed < moveSpeed)
				{
					currentMoveSpeed += 0.3f;
				}

				if (currentMoveSpeed > moveSpeed)
					currentMoveSpeed = moveSpeed;
			}
		}
		else
		{
			if (isGrounded)
			{
				//if (currentMoveSpeed > 0.1f) {
				//currentMoveSpeed -= 0.5f;
				//if(animLast != "dashBrake" && animLast != anim_land) SetAnim ("dashBrake", 0.3f);
				//} else {
				if (currentMoveSpeed > 0) currentMoveSpeed = 0;
				//if (animLast != anim_stand && animLast != anim_land && myState == plState.Ground) {
				if (animLast != anim_stand)
					if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 1)
					{
						SetAnim(anim_stand, timeStandTrns);
					}
				//}
			}
		}
	}

	void HandleHack()
	{
		if (cappy != null)
		{
			if (plsUnhack)
			{
				plsUnhack = false;
			}
			if (isHacking)
			{
				if (hasCaptured)
				{
					if (key_backR || plsUnhack)
					{
						SetCap(false);
						SetState(plState.Ground);
						cappy.hackedObj.SendMessage ("setState", 7);
						if (cappy.hackedObj.GetComponent<Collider> () != null)
						cappy.hackedObj.GetComponent<Collider> ().enabled = true;
						transform.Translate(0, 0, -2);
						ResetSpeed();
						plsUnhack = false;
						isBlocked = false;
						isBlockBlocked = false;
						isHacking = false;
						scr_main._f.capMountPoint = "";
						var Mustache = cappy.hackedObj.transform.GetChild (0);
						if (Mustache.name == "Mustache" || Mustache.name == "Mustache__HairMT")
						Mustache.gameObject.SetActive (false); //if mustache, place it at index 0
					}
				}
				else
				{
					if (!isCapturing)
					{
						isCapturing = true;
						isBlocked = true;
						SetState(plState.CaptureFly);
					}
				}
			}
			else
			{
				if (hasCaptured == true)
				{
					hasCaptured = false;
					cappy.SetState (capState.Return);
					SetVisible(true);
					SetCap(false);
				}
			}
		}
	}

	void OnTriggerEnter(Collider collis)
	{
		paramObj collisParam;
		if ((collisParam = collis.GetComponent<paramObj>()) == null) return;


		if (collisParam.isTouch)
		{
			if (hasTouchedCeiling)
			{
				collis.gameObject.SendMessage("OnTouch", 4);
				return;
			}

			if (collisParam.posCenterV != 0 && transform.position.y > collisParam.GetPosCenterV())
			{
				collis.gameObject.SendMessage("OnTouch", 3);
				return;
			}
			else if (!hasTouchedCeiling)
			{
				collis.gameObject.SendMessage("OnTouch", 2);
				return;
			}
			/*else
            {
				collis.gameObject.SendMessage("OnTouch", 0);
				return;
            }*/
		}
		/*
		 * 0 = invalid
		 * 1 = cappy collision
		 * 2 = mario normal/ lower touch collision
		 * 3 = mario higher touch collision
		 * 4 = mario touched from below(ceiling)
		 */
	}
	void OnSensorTopEnter(Collider col){
		hasTouchedCeiling = true;
	}
	void OnSensorBottomEnter(Collider col)
	{
		if (col.gameObject.layer != 20 && col.gameObject.layer != 18)
		{
			isMovingAir = false;
			isTurning = false;
			SetState(plState.Landing);
		}
	}
	void OnSensorBottomStay(Collider col)
	{
		isGrounded = true;
		if (col.gameObject.layer != 20 && col.gameObject.layer != 18)
		{ // 20 = WALL LAYER
			groundedPosition = transform.position.y;
		}
	}
	void OnSensorBottomExit(Collider col)
	{
		if (col.gameObject.layer != 20 && col.gameObject.layer != 18)
			isGrounded = false;
	}
	void OnSensorBelowStay(Collider col)
	{
		if (rb.velocity.y < 0 && !isGrounded && key_jump && col.gameObject.layer != 20 && col.gameObject.layer != 18)
			isJumpingSoon = true;
	}
	public void OnSensorLODEnter(Collider coll)
	{
		SetLOD(coll.gameObject, true);
	}
	public void OnSensorLODExit(Collider coll)
	{
		SetLOD(coll.gameObject, false);
	}
	void SetLOD(GameObject coll, bool state)
	{
		if (coll.GetComponent<paramObj>() != null && coll.GetComponent<paramObj>().isLOD)
		{
			if (coll.transform.GetChild(1).gameObject.name == "Mesh") coll.transform.GetChild(1).gameObject.SetActive(state);
			else return;
			if(coll.GetComponent<Animator>() != null) coll.GetComponent<Animator>().enabled = state;
		}
	}

	//RESET
	public void ResetSpeed()
	{
		maxJump			= 4;
		moveSpeed		= MarioTable.speedRun;
	}

	public void ResetAnim()
	{
		anim_stand 		= "idle";
		anim_run 		= "run";
		anim_runStart 	= "runStart";
		anim_land		= "landShort";
	}

	//SET
	public void SetSpeed(int _maxJump, float _moveSpeed)
	{
		maxJump = _maxJump;
		moveSpeed = _moveSpeed;
	}

	public void SetAnim(string animName, float transitionTime = -1, float standTrnsTime = 0/*, float animSpeed = 1*/)
	{
		if (!isAnim (animName))
		{
			if (transitionTime == -1) transitionTime = 0.1f; // DEFAULT VALUE
			if (transitionTime == 0 || anim.IsInTransition(0)) anim.Play(animName); // play instant
			else
			anim.CrossFade (animName, transitionTime); // play crossfade
//			anim.speed 		= animSpeed;
			animLast 		= animName;
			if(standTrnsTime!=0) timeStandTrns = standTrnsTime; //time to transition back to stand, quick of slow
		}
	}
	public void SetHand(int side, int type, bool state) // 0 = ball, 1 = flat
	{
		transform.GetChild(1).GetChild(5 + (side * 2 /* MAX NUM OF HAND TYPES */) + type).gameObject.SetActive(state);
	}
	public void SetCap(bool boolean)
	{
		transform.GetChild(1).GetChild(0).gameObject.SetActive(boolean);
		transform.GetChild(1).GetChild(1).gameObject.SetActive(!boolean);
	}
	public void SetAnimMove(string stand, string run, string runStart)
	{
		anim_runStart = runStart; anim_run = run; anim_stand = stand;
	}
	public void SetCollider(float height, float radius = 0.4f)
	{
		capsColl1.center = new Vector3(0, height / 2, 0);
		capsColl1.radius = radius;
		capsColl1.height = height;
		capsColl2.center = new Vector3(0, height / 2, 0);
		capsColl2.radius = radius;
		capsColl2.height = height;
	}
	public void SetVisible(bool boolean)
    {
		for (int i = 0; i < transform.GetChild(1).childCount; i++)
        {
			transform.GetChild(1).GetChild(i).gameObject.SetActive(boolean);
        }
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

	// Bezier function
	Vector3 Bezier(Vector3 a, Vector3 b, Vector3 c, float t)
	{
		return Vector3.Lerp(Vector3.Lerp(a, b, t), Vector3.Lerp(b, c, t), t);
	}
}
