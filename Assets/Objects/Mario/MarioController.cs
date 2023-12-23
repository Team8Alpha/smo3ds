using UnityEngine;
using System.Collections;
using System;

public enum eStatePl
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
	[HideInInspector] public static eStatePl myState;
	[HideInInspector] public int mySubState = 0;

	public byte maxJump = 6;
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
	
	[HideInInspector] public bool key_jump 			= false;
	[HideInInspector] public bool key_backL 		= false;
	[HideInInspector] public bool key_backR			= false;
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
	[HideInInspector] public static MarioController s;

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
	[HideInInspector] byte jumpAfterTimer 			= 0; //timer till it refuses to execute double jump
	[HideInInspector] bool hasTouchedCeiling 		= false;
	[HideInInspector] float lastGroundedPosition 	= 0;
	[HideInInspector] private bool isMovingAir 		= false; //if falling direction is active
	[HideInInspector] private float speedJumpH; //used for falling direction
	[HideInInspector] private Vector3 lastPosition;
	[HideInInspector] float timeStandTrns = 0.5f;
    [HideInInspector] bool[] meshPartsVisible = { true, true, false, true, false };
												// cap, haLb, haLf,  haRb, haRf

    [HideInInspector] CapsuleCollider capsColl1;
	[HideInInspector] CapsuleCollider capsColl2;
	//jump force max,  jump height min,  jump height max

	void Awake()
	{
		scr_main.s.GetComponent<AudioListener>().enabled = false;
		GetComponent<AudioListener>().enabled = true;

		// Store references to Animator and Rigidbody components
		anim = GetComponent<Animator>();
		rb = GetComponent<Rigidbody>();
		capsColl1 = GetComponents<CapsuleCollider>()[0];
		capsColl2 = GetComponents<CapsuleCollider>()[1];

		groundedPosition = transform.position.y;
		lastPosition = transform.position;

		resetVisibleParts();

		s = this;
	}

	void OnDestroy()
    {
		if(scr_main.s != null) scr_main.s.GetComponent<AudioListener>().enabled = true;
	}

	void Start()
    {
		SetState(eStatePl.Ground);
    }

	void Update()
	{

		if (scr_main.s.isFocused)
		{
			HandleInput();
			HandleMove();

			// Update Mario's animation and movement based on states
			switch (myState)
			{
				case eStatePl.Ground: // Standing still, wait

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
						SetState(eStatePl.Falling);
					break;

				case eStatePl.Jumping: // Jumping from land normal
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
								SetState(eStatePl.Landing);
							break;
						case 1:
							rb.AddForce(Vector3.down * MarioTable.dataJump[jumpType - 1][0] * 5 * jumpedHeight, ForceMode.Acceleration);
							if (hasTouchedCeiling)
							{
								rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
								jumpAfterTimer = 0;
							}
							if (rb.velocity.y < 0)
								SetState(eStatePl.Falling, 1); //substate 1, jumpfall
							break;
					}
					break;
				case eStatePl.Falling: //FALLING CAM
					switch (mySubState)
					{
						case 0: //camera follow
							groundedPosition = transform.position.y;
							if (transform.position.y < lastGroundedPosition - 4)
							{
								if (transform.position.y > lastGroundedPosition - 5)
								{
									MarioCam.s.confSmoothTime = 10f;

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
						SetState(eStatePl.Landing);
						MarioCam.s.ResetValue();
					}
					break;

				case eStatePl.Landing: //land
					SetState(eStatePl.Ground);
					break;

				case eStatePl.CaptureFly: //flying to capture enemy
					// Calculate the current percentage of the journey completed
					float distanceCovered = (Time.time - hackFlyStartTime) * hackFlyLength / 1;
					float journeyFraction = distanceCovered / hackFlyLength;

					// Calculate the current position along the Bezier curve
					Vector3 posHackObj = cappy.hackedObj.transform.position;
					Vector3 targetPosition = Bezier (transform.position, posHackObj + Vector3.up * 2 + (transform.position - posHackObj).normalized * 1, posHackObj, journeyFraction);

					// Calculate the additional movement vector based on the target position
					transform.Translate(targetPosition - transform.position);

					// Check if the movement is completed
					if (Vector3.Distance(transform.position, posHackObj) < 1f)
					{
						transform.position = posHackObj;
						moveAdditional = Vector3.zero;
						MarioEvent.s.SetEvent(eEventPl.hack, 1);
					}
					break;
				case eStatePl.Squat:
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
		Vector3 movementVector = transform.rotation * Vector3.forward * currentMoveSpeed;// * Time.deltaTime; //mashed together movement math
		movementVector.y = isFreezeFall ? 0 : yVel; //reassign old yvel
		movementVector += moveAdditional;

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
				case eStatePl.Ground:
					if (key_jump || isJumpingSoon)
					{
						if (!lockJump)
						{
							lockJump = true;
							isJumpingSoon = false;
							SetState(eStatePl.Jumping);
						}
						else if (!key_jump)
							lockJump = false;
					}
					else if (lockJump)
						if (!key_jump)
							lockJump = false;

					if (key_backR)
						SetState(eStatePl.Squat);
					break;

				case eStatePl.Jumping:
					if (!key_jump)
						lockJump = false;
					break;

				case eStatePl.Squat:
					if (!key_backR)
					{
						moveAdditional = Vector3.zero;
						SetState(eStatePl.Ground);
					}
					break;
			}
		}
	}

	public void SetState(eStatePl state, int subState = 0)
	{
		myState = state;
		mySubState = subState;
		switch (state)
		{
			case eStatePl.Ground:

				//reset some data
				SetCollider(1.6f);
				ResetSpeed();
				ResetAnim();
				MarioCam.s.ResetValue();

				currentTurnSpeed = MarioTable.speedTurnWalk;
				isInstTurn = false;
				break;

			case eStatePl.Jumping:
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

			case eStatePl.Falling:
				switch (subState)
				{
					case 0://falling, below lastgroundedposition
						lastGroundedPosition = groundedPosition;
						MarioCam.s.confSmoothTime = 0.2f;
						MarioCam.s.confYOffset = 1;
						MarioCam.s.confCamDistance = MarioCam.s.defCamDistance - 1;
						SetAnim("falling", 0.1f);
						break;
					case 1://falling after jump, still above lastgroundedposition
						break;
				}

				currentTurnSpeed = MarioTable.speedTurnFall;
				isInstTurn = true;

				break;

			case eStatePl.Landing: //TODO: FALLING-LANDING HEIGHT STUFF
				//SetAnim(anim_land, 0.02f); // WIP LANDING HEIGHTS

				hasJumped = false;
				break;

			case eStatePl.CaptureFly:
				hackFlyStartTime = Time.time;
				SetAnim("captureFly");
				break;

			case eStatePl.Squat:
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
			if (!isMovingAir && myState == eStatePl.Jumping)
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
						+ Mathf.Atan2(MarioCam.s.transform.forward.x, MarioCam.s.transform.forward.z) * Mathf.Rad2Deg;
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
				//if (animLast != anim_stand && animLast != anim_land && myState == eStatePl.Ground) {
				if (animLast != anim_stand)
					if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 1)
					{
						SetAnim(anim_stand, timeStandTrns);
					}
				//}
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
			SetState(eStatePl.Landing);
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
		int iPart = (side * 2 /* MAX NUM OF HAND TYPES */) + type;
		transform.GetChild(1).GetChild(5 + iPart).gameObject.SetActive(state);
		meshPartsVisible[iPart + 1] = state;
	}
	public void SetCap(bool boolean)
	{
		transform.GetChild(1).GetChild(0).gameObject.SetActive(boolean); meshPartsVisible[0] = boolean;
		transform.GetChild(1).GetChild(1).gameObject.SetActive(!boolean);
	}
	public void SetVisible(bool boolean)
	{
		if (boolean)
		{
			transform.GetChild(1).GetChild(2).gameObject.SetActive(true);
			transform.GetChild(1).GetChild(4).gameObject.SetActive(true);
			transform.GetChild(1).GetChild(9).gameObject.SetActive(true);
			resetVisibleParts();
		}
		else for (int i = 0; i != transform.GetChild(1).childCount; i++)
			transform.GetChild(1).GetChild(i).gameObject.SetActive(false);
	}
	void resetVisibleParts()
	{
		SetCap(meshPartsVisible[0]);
		SetHand(0, 0, meshPartsVisible[1]);
		SetHand(0, 1, meshPartsVisible[2]);
		SetHand(1, 0, meshPartsVisible[3]);
		SetHand(1, 1, meshPartsVisible[4]);
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
