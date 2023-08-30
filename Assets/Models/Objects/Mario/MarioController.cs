using UnityEngine;
using System.Collections;
using System;

public class MarioController : MonoBehaviour
{

	private float h=0;
	private float v=0;
	public Animator anim;
	public float moveSpeed = 11f;
	private float currentMoveSpeed = 0;
	public float jumpForce = 10;
	private float velocity; // jump velo
	public Transform target; // Target which changes position based on floor collision
	public scr_behaviorMarioCap cappy;

	private CharacterController controller;
	public float groundCheckDistance = 0.1f;
	public bool isGrounded = false;
	public float groundedPosition = 0; // latest floor position
	private float camYOffset = 4f;
	public bool isMoving = false;
	private float walkRotation = 0f; // stores the walk rotation offset or something
	private float walkRotationOffset = 3f; // rotates marios walking direction, so he runs in circles. (ITS OFFSET)
	public int jumpAct = 0;
	private float jumpedHeight = 1;
	public static MarioController marioObject;
	public bool hasCaptured = false;
	private bool isCapturing = false;
	private bool hasJumped = false;
	private float jumpedTime = 0f;
	private RaycastHit hit;
	private float fvar0 = 0; // temporary var
	public bool isBlocked = false;
	public bool isHacking = false; // hack = modify/take control of object
	public bool isBlockBlocked = false; // to prevent it from setting block to false, if it handles multiple blocks...
	public bool plsUnhack = false;
	public int maxJump = 10;
	public string animLast = "wait";

	private void Awake()
	{
		scr_gameInit.globalValues.GetComponent<AudioListener> ().enabled = false;
		GetComponent<AudioListener> ().enabled = true;
		jumpedHeight -= 0;
		hit.GetType ();
		marioObject = this;
		anim = GetComponent<Animator> ();
		controller = GetComponent<CharacterController> ();
	}

	private void OnTriggerEnter(Collider collis)
	{
		try {
			if (collis.gameObject.layer != scr_gameInit.lyr_def) {
				if (collis.gameObject.layer == scr_gameInit.lyr_enemy || collis.gameObject.layer == scr_gameInit.lyr_obj) {
					if (transform.position.y < collis.GetComponent<paramObj> ().bCenterY ()) {
						collis.gameObject.SendMessage ("OnTouch", 2);
					} else {
						collis.gameObject.SendMessage ("OnTouch", 3);
					}
				}
			}
		} catch (Exception e) {
			// Handle exceptions if needed
		}
	}

	private void FixedUpdate()
	{
		if (scr_gameInit.globalValues.isFocused) {

			// Ground check using CharacterController's isGrounded
			isGrounded = controller.isGrounded;

			// Get input values
			#if UNITY_EDITOR
			h = Input.GetAxisRaw ("Horizontal");
			v = Input.GetAxisRaw ("Vertical");
			#else
			h = UnityEngine.N3DS.GamePad.CirclePad.x;
			v = UnityEngine.N3DS.GamePad.CirclePad.y;
			#endif

			// Check if Mario is blocked
			if (isBlocked) {
				h = 0;
				v = 0;
				velocity = 0;
				jumpAct = 3;
			}

			isMoving = h != 0 || v != 0;

			// Handle movement
			if (isMoving) {
				HandleMovement ();
				if (animLast == "wait")
					setAnim ("runStart"); //yeah goodnight
			}

			// Handle jumping
			HandleJumping ();

			// Handle falling
			HandleFalling ();

			// Handle Hacking
			HandleHacking ();
		}
	}

	private void HandleMovement()
	{
		float tmp_walkRotation = 0;
		if (transform.rotation.y < 179 && transform.rotation.y > -179) {
			// adjust angle
			if (tmp_walkRotation < -90) {
				tmp_walkRotation += 360;
			} else if (tmp_walkRotation > 90) {
				tmp_walkRotation -= 360;
			}
					
			walkRotation += tmp_walkRotation / 76;
			tmp_walkRotation = Mathf.Atan2 (h, v) * Mathf.Rad2Deg;
		} else {
			tmp_walkRotation = 0; 
		}
		transform.rotation = Quaternion.Euler (transform.eulerAngles.x, tmp_walkRotation + walkRotation + MarioCam.marioCamera.gameObject.transform.eulerAngles.y, transform.eulerAngles.z);
		if (currentMoveSpeed < moveSpeed)
			currentMoveSpeed += 0.1f;
		if (h < 0f)
			h = h * -1;
		if (v < 0f)
			v = v * -1;
	}

	private void HandleJumping()
	{
		// Jump if the jump button is pressed and the object is grounded
		if (isGrounded) {
			groundedPosition = transform.position.y;
			#if UNITY_EDITOR
			if (!hasJumped && Input.GetKey (KeyCode.Space)) {
				jumpAct = 1;
			}
			#else
			if (!hasJumped && (UnityEngine.N3DS.GamePad.GetButtonHold(N3dsButton.A) || UnityEngine.N3DS.GamePad.GetButtonHold(N3dsButton.B)))
			{
			jumpAct = 1;
			}
			#endif
		}

		switch (jumpAct) {
		case 0:
			if (hasJumped) {
				#if UNITY_EDITOR
				if (!Input.GetKey (KeyCode.Space)) {
					hasJumped = false;
				}
				#else
				if (!UnityEngine.N3DS.GamePad.GetButtonHold(N3dsButton.A) && !UnityEngine.N3DS.GamePad.GetButtonHold(N3dsButton.B))
				{
				hasJumped = false;
				}
				#endif
			}
			velocity = 0;
			break;
		case 1:
			hasJumped = true;
			jumpedTime++;
			if (jumpedTime == 1) {
				velocity = 0.8f;
				setAnim ("jump");
			}
			velocity += jumpForce;
			if ((jumpedTime > maxJump && (!UnityEngine.N3DS.GamePad.GetButtonHold (N3dsButton.A) && !Input.GetKey (KeyCode.Space) && !UnityEngine.N3DS.GamePad.GetButtonHold (N3dsButton.B))) || jumpedTime > maxJump * 2) {
				jumpAct = 2;
				jumpedHeight = transform.position.y;
				fvar0 = jumpedTime;
				jumpedTime = 0;
			}
			break;
		case 2:
			jumpedTime++;
			velocity += 0.1f;
			if (jumpedTime > 6 + fvar0 * 0.4f) {
				jumpAct = 3;
			}
			break;
		case 3:
			if (isGrounded) {
				jumpAct = 0;
				jumpedTime = 0;
				if (isMoving) {
					setAnim ("run", 0.1f);
				} else {
					setAnim ("default", 0.1f);
				}
			} else {
				velocity = -0.7f;
			}
			break;
		}
	}

	private void HandleFalling()
	{
		if (!isMoving && currentMoveSpeed > 0) {
			currentMoveSpeed -= 0.2f;
		}

		// Calculate the movement vector based on the input and current speed
		Vector3 movementVector = new Vector3 (0f, 0f, h + v) * currentMoveSpeed * Time.deltaTime;

		// Move the character using the CharacterController
		controller.Move ((transform.rotation * Vector3.forward) * movementVector.magnitude + new Vector3 (0, velocity * (Time.deltaTime * 6), 0));
	}
	private void HandleHacking()
	{	
		if (cappy != null) {
			if (plsUnhack) {
				plsUnhack = false;
			} else {
				if (cappy.currentState == 3)
				if (UnityEngine.N3DS.GamePad.GetButtonHold (N3dsButton.Y) || UnityEngine.N3DS.GamePad.GetButtonHold (N3dsButton.X) || Input.GetKey (KeyCode.LeftShift)) {
					cappy.SetState (0); //cappy handles everything.
				}
			}
		}
		if (isHacking) {
			if (hasCaptured) {
				if (UnityEngine.N3DS.GamePad.GetButtonHold (N3dsButton.L) || UnityEngine.N3DS.GamePad.GetButtonHold (N3dsButton.R)
				     || Input.GetKey (KeyCode.Y) || plsUnhack) {
					plsUnhack = false;
					transform.GetChild (2).gameObject.SetActive (false);//hair
					transform.GetChild (1).gameObject.SetActive (true);//cap
					setAnim ("default");
					cappy.capturedObject.SendMessage ("setState", 7);
					cappy.capturedObject.tag = "Untagged";
					velocity = 4;
					transform.Translate (0, 0, -2);
					ResetSpeed ();
					isBlocked = false;
					isBlockBlocked = false;
					isHacking = false;
					scr_gameInit.globalValues.capMountPoint = "missingno";
					var Mustache = cappy.capturedObject.transform.GetChild (0);
					if (Mustache.name == "Mustache" || Mustache.name == "Mustache__HairMT") 
						Mustache.gameObject.SetActive (false); //if mustache, place it at index 0
				}
			} else {
				if (!isCapturing) {
					isCapturing = true;
					isBlocked = true;
					setAnim ("captureFly");
					jumpAct = 1;
				} else {
					if (anim.GetCurrentAnimatorStateInfo (2).IsName ("captureFly")) {
						transform.position = Vector3.MoveTowards (transform.position, cappy.capturedObject.transform.position, 0.3f);
					} else {
						isCapturing = false;
						if (!isBlockBlocked)
							isBlocked = false;
						hasCaptured = true;
						transform.position = cappy.capturedObject.transform.position;
						cappy.capturedObject.SendMessage ("setState", 6);
						for (int i = 0; i <= 8; i++) {
							transform.GetChild (i).gameObject.SetActive (false);
						}
					}
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



	public void ResetSpeed()
	{
		maxJump = 10;
		moveSpeed = 5.5f;
	}

	public void SetSpeed(int _maxJump, float _moveSpeed, float scaleCap = 1)
	{
		maxJump = _maxJump;
		moveSpeed = _moveSpeed;
		cappy.transform.localScale = new Vector3 (scaleCap, scaleCap, scaleCap);
	}

	public void setAnim(string animName, float transitionTime = 0.15f, float animSpeed = 1)
	{
		if (isAnim (animName)) {
			anim.CrossFade (animName, transitionTime);
			anim.speed = animSpeed;
			animLast = animName;
		}
	}

	public bool isAnim(string anmName)
	{
		try {
			return !anim.GetCurrentAnimatorStateInfo (0).IsName (anmName) && !anim.GetCurrentAnimatorStateInfo (1).IsName (anmName);
		} catch (Exception e) {
			return !anim.GetCurrentAnimatorStateInfo (0).IsName (anmName);
		}
	}

}