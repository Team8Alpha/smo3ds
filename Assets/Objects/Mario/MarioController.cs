using UnityEngine;
using System.Collections;
using System;

public enum eStatePl
{
	Ground,
	Jumping,
	Falling,
	Landing,
	Squat,
	GroundPound,
	WallJump
}

public class MarioController : MonoBehaviour
{
	[HideInInspector] public static eStatePl myState;
	[HideInInspector] public int mySubState = 0;

	public float moveSpeed = 8.06f;


	[HideInInspector] public bool key_jump			= false;
	[HideInInspector] public bool key_backL			= false;
	[HideInInspector] public bool key_backR			= false;
	[HideInInspector] public bool key_cap			= false;

	[HideInInspector] public bool wasGrounded 		= true; // is colliding with floor
	[HideInInspector] public bool isMoving 			= false; // is pressing move buttons
	[HideInInspector] public bool wasMoving 		= false; // was moving in last frame
	[HideInInspector] public bool lockJump			= false; // locks jump for behavior stuff
	[HideInInspector] public bool hasJumped 		= false; // if has jumped prev.
	[HideInInspector] public bool isJumpingSoon		= false; // if has pressed jump button just before landing
	[HideInInspector] public bool isInstTurn		= false; 
	[HideInInspector] public bool isTurning			= false; 
	[HideInInspector] public bool isInputBlocked	= false; // if process input functions or not
	[HideInInspector] public bool isHacking			= false; // if is capturing, hacking...
	[HideInInspector] public bool isRunReady		= true;
	[HideInInspector] public bool isFlyFreeze		= false;
	[HideInInspector] bool isMovingAir				= false; // if falling direction is active, falling
	[HideInInspector] bool hasTouchedCeiling		= false; // has touched top

	[HideInInspector] public float currentMoveSpeed = 0; // varying move speed

	[HideInInspector] public Animator anim;
	[HideInInspector] public Rigidbody rb;

	float h											= 0; // controller y
	float v 										= 0; // controller x
	float currentTurnSpeed							= 0;
	float currentRotation							= 0;
	float speedSlip									= 0; // slipping speed
	float posLastGround								= 0;
	float speedJumpH								= 0; //used for falling direction
	float posLastHigh								= 0;
	float timeStandTrns								= 0.5f;
	byte jumpAfterTimer								= 0; //timer till it refuses to execute double jump
	Vector3 lastPosition;
	//							 cap, haLb, haLf,  haRb, haRf
	bool[] meshPartsVisible = { true, true, false, true, false };

	[HideInInspector] public scrBehaviorCappy		cappy;
	[HideInInspector] public static MarioController s;

	[HideInInspector] public float posGround		= 0; // latest floor position
	[HideInInspector] public byte jumpType			= 0;
	[HideInInspector] public Vector3 moveAdditional = Vector3.zero;
	[HideInInspector] public string animLast 		= "idle";

	[HideInInspector] CapsuleCollider[] colTrigger;

	void Awake()
	{
		scr_main.s.GetComponent<AudioListener>().enabled = false;
		GetComponent<AudioListener>().enabled = true;

		// Store references to Animator and Rigidbody components
		anim = GetComponent<Animator>();
		rb = GetComponent<Rigidbody>();
		colTrigger = GetComponents<CapsuleCollider>();

		posGround = transform.position.y;
		lastPosition = transform.position;

		isFlyFreeze = false;

		resetVisibleParts();

		s = this;

		transform.Translate(0, 0.2f, 0);

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
					if (!wasGrounded) SetState(eStatePl.Falling, 2);
					//TODO: FALLING WHEN NOT GROUNDED
                    break;

				case eStatePl.Jumping: // Jumping from land normal
					float jumpedHeight = (transform.position.y - posLastGround) * 1.4f;
					switch (mySubState)
					{
						case 0:

							if ((jumpedHeight > MarioTable.dataJump[jumpType - 1][1] && !key_jump)
								|| (jumpedHeight > MarioTable.dataJump[jumpType - 1][2] && key_jump)
								|| hasTouchedCeiling)
							{

								mySubState = 1;
								posLastHigh = transform.position.y;

							}
							//once left ground, he jumped.
							if (!wasGrounded)
								hasJumped = true;
							//if clipped up without even starting the jumpfall, he landed.
							if (wasGrounded && hasJumped)
								SetState(eStatePl.Landing);
							break;
						case 1:
							rb.AddForce(Vector3.down * MarioTable.dataJump[jumpType - 1][0] * 12, ForceMode.Acceleration);
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
					if (wasGrounded)
					{
						SetState(eStatePl.Landing);
						MarioCam.s.ResetValue();
					}
					switch (mySubState)
					{
						case 0: //camera follow
							posGround = transform.position.y;
							if (transform.position.y < posLastGround - 4)
							{
								if (transform.position.y > posLastGround - 5)
								{
									MarioCam.s.confSmoothTime = 10f;

								}
							}
							break;
						case 1://jump fall
							if (transform.position.y < posLastGround - 3)
								SetState(myState, 0);
							break;
						case 2:
							if (transform.position.y < posGround - 3)
								SetState(eStatePl.Falling);
							if (wasGrounded) SetState(eStatePl.Landing);
							break;
					}
					break;

				case eStatePl.Landing: //land
					SetState(eStatePl.Ground, 1);
					break;

				case eStatePl.Squat:
                    switch (mySubState) {
						case 0:
							if (rb.velocity.magnitude < 0.1f)
							{
								isInputBlocked = false;
								SetState(myState, 1);
							}
							break;
						case 1:
							break;
					}
					break;
			}
		}
		hasTouchedCeiling = false;

		lastPosition = transform.position;
		wasMoving = isMoving;
	}

	void HandleInput()
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
			if (key_backL)
			{
				h = 0; v = 0;
				return;
			} else {

				h = UnityEngine.N3DS.GamePad.CirclePad.x;
				v = UnityEngine.N3DS.GamePad.CirclePad.y;

				key_jump = UnityEngine.N3DS.GamePad.GetButtonHold(N3dsButton.A) || UnityEngine.N3DS.GamePad.GetButtonHold(N3dsButton.B);
				key_cap = UnityEngine.N3DS.GamePad.GetButtonHold(N3dsButton.X) || UnityEngine.N3DS.GamePad.GetButtonHold(N3dsButton.Y);
			}
#endif

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
						if (isHacking) jumpType = 0;
						SetState(eStatePl.Jumping);
					}
					else if (!key_jump)
						lockJump = false;
				}
				else if (lockJump)
					if (!key_jump)
						lockJump = false;

				if (!isHacking) if (key_backR)
					SetState(eStatePl.Squat);
				break;

			case eStatePl.Jumping:
				if (!key_jump)
					lockJump = false;
				break;

			case eStatePl.Squat:
				if (!key_backR)
				{
					SetState(eStatePl.Ground); //TODO: standup anim
					isInputBlocked = false;
				}
				break;
			case eStatePl.Falling:
				if (key_jump && Physics.Raycast(transform.position, -Vector3.up, 0.8f)) isJumpingSoon = true;
				break;
		}
	}

	public void SetState(eStatePl state, int subState = 0)
	{
		myState = state;
		mySubState = subState;
		switch (state)
		{
			case eStatePl.Ground:

                switch (subState)
                {
					case 0:
						//reset some data
						SetCollider(1.6f);
						ResetSpeed();
						MarioCam.s.ResetValue();

						currentTurnSpeed = MarioTable.speedTurnWalk;
						isInstTurn = false;
						break;
					case 1:
						SetState(eStatePl.Ground);
						break;
                }
				break;

			case eStatePl.Jumping:
				jumpType++;
				jumpAfterTimer = 1;
				float timeTrnsJump = 0.08f;
				float timeTrasStand = 0.2f;
				switch (jumpType)
				{
					case 2:
						SetAnim("jump2", timeTrnsJump, timeTrasStand);
						break;
					case 3:
						if (currentMoveSpeed < 6)
						{
							SetAnim("jump", timeTrnsJump, timeTrasStand);
							jumpType = 1;
							break;
						}
						SetAnim("jump3", timeTrnsJump, timeTrasStand);
						break;
					default:
						SetAnim("jump", timeTrnsJump, timeTrasStand);
						jumpType = 1;
						break;
				}

				currentTurnSpeed = MarioTable.speedTurnJump;
				posLastGround = posGround;
				isInstTurn = true;

				rb.AddForce(MarioTable.dataJump[jumpType - 1][0] * Vector3.up * 1.4f, ForceMode.Impulse);
				break;

			case eStatePl.Falling:
				switch (subState)
				{
					case 0://falling, below posLastGround
						posLastGround = posGround;
						MarioCam.s.confSmoothTime = 0.2f;
						MarioCam.s.confYOffset = 1;
						MarioCam.s.confCamDistance = MarioCam.s.defCamDistance - 1;
						break;
					case 1://falling after jump, still above posLastGround
						break;
					case 2://pos lower than last ground, short fall
						SetAnim("falling", 0.7f);
						break;
				}

				currentTurnSpeed = MarioTable.speedTurnFall;
				isInstTurn = true;

				break;

			case eStatePl.Landing: //TODO: FALLING-LANDING HEIGHT STUFF
				float trnsLand = 0.1f;
				float height = Mathf.Abs(posLastHigh - transform.position.y);
				if (!isMoving)
				{
					if (height > 2)
					{
						if (height > 6)
						{
							SetAnim("landDownFall", trnsLand);
						}
						else
						{
							SetAnim("landShort", trnsLand);
						}
					}
					else isRunReady = true;
				} else
                {
					isRunReady = true;
                }
				

				hasJumped = false;
				break;

			case eStatePl.Squat:
				SetCollider(0.94f);
				if (isMoving)
					SetAnim("squatStart_w", 0.1f);
				else
					SetAnim("squatStart_s", 0.1f);

				moveAdditional = transform.rotation * Vector3.forward * (currentMoveSpeed / 50);

				currentTurnSpeed = MarioTable.speedTurnSquat;
				isInputBlocked = true;

				speedSlip = 0.6f;

				break;
		}

	}


	void HandleMove()
	{
		float angleSpeed = 0;
		RaycastHit hit;

		// Perform a SphereCast to check the slope angle if the character was grounded
		if (Physics.SphereCast(colTrigger[0].center + transform.position, 0.2f, -Vector3.up, out hit, 0.8f))
		{
			angleSpeed = Vector3.Dot(transform.forward, Vector3.Cross(Vector3.up, Vector3.Cross(Vector3.up, hit.normal)));

			// Check if the angleSpeed is within the slope threshold to consider it as grounded
			if (Mathf.Abs(angleSpeed) <= 75) // MAX ANGLE
			{
				if (!wasGrounded)
				{
					isMovingAir = false;
					isTurning = false;
					posGround = transform.position.y;
					SetState(eStatePl.Landing);
					wasGrounded = true;
					Debug.Log("M Ground Enter");
				}
			}
			else
			{
				if (wasGrounded)
				{
					wasGrounded = false;
					posGround = transform.position.y;
					posLastHigh = transform.position.y;
					Debug.Log("M Ground Exit");
				}
			}
		}
		else
		{
			if (wasGrounded)
			{
				wasGrounded = false;
				posGround = transform.position.y;
				posLastHigh = transform.position.y;
				Debug.Log("M Ground Exit");
			}
		}

		moveAdditional = Vector3.zero;
		if (!wasGrounded)
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
		} else
        {
			HandleMoveAnim();
		}

		if ((isMoving || isMovingAir) && !isInputBlocked )
		{
			if (wasGrounded)
			{
                if (!wasMoving)
                {
					isRunReady = true;
                }
				posGround = transform.position.y;

				// Adjust acceleration based on angleSpeed
				float adjustedAcceleration = 10;
				if (angleSpeed < 0)
				{
					// Moving up, decrease acceleration
					adjustedAcceleration *= (1 - Mathf.Abs(angleSpeed) / 180f);
				}
				else
				{
					// Moving down, increase acceleration
					adjustedAcceleration *= (1 + Mathf.Abs(angleSpeed) / 90f);
				}

				currentMoveSpeed = Mathf.MoveTowards(currentMoveSpeed, moveSpeed, adjustedAcceleration * Time.deltaTime);
			}

			{
				if (/*transform.rotation.y < 179 && transform.rotation.y > -179 && */!isTurning)
				{
					if(isMoving) currentRotation = Mathf.Atan2(h, v) * Mathf.Rad2Deg
						+ Mathf.Atan2(MarioCam.s.transform.forward.x, MarioCam.s.transform.forward.z) * Mathf.Rad2Deg;
				}

					float angleDistance = Mathf.Abs(Mathf.DeltaAngle(transform.eulerAngles.y, currentRotation));
				if (isTurning)
				{
					if (angleDistance > 100) currentTurnSpeed = Mathf.Abs(currentTurnSpeed) * 2;
					else if (angleDistance < 1) isTurning = false;
				}
				else
				{
					if (angleDistance > 100)
					{
						isTurning = true;
						if (wasGrounded)
						{
							SetAnim("turn", 0.06f);
						}
					}
				}

				transform.rotation = Quaternion.Euler(transform.eulerAngles.x,
					Mathf.MoveTowardsAngle(transform.eulerAngles.y, currentRotation, currentTurnSpeed), 
					transform.eulerAngles.z);
			}
		}
		else
		{
			if (wasGrounded)
			{
				if (wasMoving) isRunReady = true;
				//if (currentMoveSpeed > 0.1f) {
				//currentMoveSpeed -= 0.5f;
				if (currentMoveSpeed > 0 || currentMoveSpeed < 0) currentMoveSpeed = speedSlip > 0 && currentMoveSpeed > 0 ? currentMoveSpeed - speedSlip : 0;
			}
		}

		Vector3 movementVector = transform.rotation * Vector3.forward * (currentMoveSpeed - (angleSpeed * (currentMoveSpeed / 2)));// * Time.deltaTime; //mashed together movement math
		if (isFlyFreeze) movementVector.y = 0; else movementVector.y += rb.velocity.y;
		movementVector += moveAdditional;

		// Move the character using the Rigidbody
		rb.velocity = movementVector * Time.deltaTime * 1 / Time.deltaTime;
	}

	void HandleMoveAnim()
	{
		//Debug.Log(rb.velocity.magnitude);
		//Debug.Log(anim.GetCurrentAnimatorStateInfo(0).normalizedTime);
		if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 1) isRunReady = true; // some number that isnt any run type

		if (isRunReady)
		{
			float velSpeed = Mathf.Sqrt(rb.velocity.x * rb.velocity.x + rb.velocity.z * rb.velocity.z);
			if (velSpeed > 7)
			{
				SetAnim("dashFast");
			}
			else if (velSpeed > 0.1f)
			{
				SetAnim("run");
			}
			/*else if (velSpeed > 0)
			{
				SetAnim("runStart");
			}*/
			else
			{
				SetAnim("idle");
			}
			isRunReady = false;
		}

	}

	void OnTriggerEnter(Collider collis)
	{
		paramObj collisParam;
		if ((collisParam = collis.gameObject.GetComponent<paramObj>()) == null) return;


		if (collisParam.isTouch)
		{
			if (hasTouchedCeiling)
			{
				collis.gameObject.SendMessage("OnTouch", 4);
			}
			else if (collisParam.posCenterV != 0 && transform.position.y > collisParam.GetPosCenterV())
			{
				collis.gameObject.SendMessage("OnTouch", 3);
			}
			else
			{
				collis.gameObject.SendMessage("OnTouch", 2);
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
	public void OnSensorLODEnter(Collider coll)
	{
		SetLOD(coll.gameObject, true);
	}
	public void OnSensorLODExit(Collider coll)
	{
		SetLOD(coll.gameObject, false);
	}
	public void SetLOD(GameObject coll, bool state)
	{
		if (coll.GetComponent<paramObj>() != null && coll.GetComponent<paramObj>().isLOD)
		{
			if (coll.transform.GetChild(1).gameObject.name == "Mesh") coll.transform.GetChild(1).gameObject.SetActive(state);
			else { Debug.Log("E: INVALID MESH TREE AT " + coll.name); return; }
			if (coll.GetComponent<Animator>() != null) coll.GetComponent<Animator>().enabled = state;
			if (coll.GetComponent<AudioSource>() != null) coll.GetComponent<AudioSource>().enabled = state;
			if (coll.GetComponent<Rigidbody>() != null)
            {
				if (state) coll.GetComponent<Rigidbody>().WakeUp();
				else coll.GetComponent<Rigidbody>().Sleep();
			}
		}
	}

	//RESET
	public void ResetSpeed()
	{
		moveSpeed		= MarioTable.speedRun;
	}

	//SET
	public void SetSpeed(byte _maxJump, float _moveSpeed)
	{
		moveSpeed = _moveSpeed;
	}

	public void SetAnim(string animName, float transitionTime = -1, float standTrnsTime = -1, bool hasExitTime = true/*, float animSpeed = 1*/)
	{
		if (!isAnim (animName))
		{
			if (transitionTime == -1) transitionTime = 0.3f; // DEFAULT VALUE
			if (transitionTime == 0 || anim.IsInTransition(0)) anim.Play(animName); // play instant
			else anim.CrossFade (animName, transitionTime); // play crossfade

			//anim.speed 		= animSpeed;
			animLast 		= animName;
			if(standTrnsTime != -1) timeStandTrns = standTrnsTime; //time to transition back to stand, quick of slow
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
	public void SetCollider(float height)
	{
		Vector3 center = new Vector3(0, height / 2, 0);
		//Vector3 size = new Vector3(colTrigger.size.x, height, colTrigger.size.z);
		foreach(CapsuleCollider coll in colTrigger)
		{
			coll.center = center;
			coll.height = height;
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
}
