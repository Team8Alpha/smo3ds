using UnityEngine;
using System;

public class MarioController : MonoBehaviour
{
	public Animator anim;
	public float moveSpeed = 11f;
	private float currentMoveSpeed = 0;
	public float jumpForce = 10;
	float velocity; //jump velo
	public Transform target; // Target which changes position based on floor collision
	public scr_behaviorMarioCap cappy;

	private Rigidbody rb;
	public float groundCheckDistance = 0.1f;
	public bool isGrounded = false;
	public float groundedPosition = 0; // latest floor position
	public float camYOffset = 4f;
	public bool isMoving = false;
	private float walkRotation = 0f; // stores the walk rotation offset or something
	public float walkRotationOffset = 3f; //rotates marios walking direction, so he runs in circles. (ITS OFFSET)
	public int jumpAct = 0;
	private float jumpedHeight = 1;
	public static MarioController marioObject;
	public bool hasCaptured = false;
	private bool isCapturing = false;
	private bool hasJumped = false;
	private float jumpedTime = 0f;
	private RaycastHit hit;
	private float fvar0 = 0; //temporary var
	public bool isBlocked = false;
	public bool isHacking = false; //hack = modify/take control of object
	public bool isBlockBlocked = false; //to prevent it from setting block to false, if it handles multiple blocks...
	public bool plsUnhack = false;
	public int maxJump = 10;

	public void ResetSpeed() {
		maxJump = 10;
		moveSpeed = 5.5f;
	}
	public void SetSpeed(int _maxJump, float _moveSpeed, float scaleCap = 1){
		maxJump = _maxJump;
		moveSpeed = _moveSpeed;
		cappy.transform.localScale = new Vector3 (scaleCap, scaleCap, scaleCap);
	}

	void Awake(){
		scr_gameInit.globalValues.GetComponent<AudioListener> ().enabled = false;
		GetComponent<AudioListener> ().enabled = true;
		jumpedHeight -= 0;
		hit.GetType ();
		marioObject = this;
		anim = GetComponent<Animator> ();
		rb = GetComponent<Rigidbody> ();
	}

	public void setAnim(string animName, float transitionTime = 0.15f) {
		if(isAnim(animName)) anim.CrossFade (animName, transitionTime);
	}
	public bool isAnim(string anmName) {
		return !anim.GetCurrentAnimatorStateInfo (0).IsName (anmName) && !anim.GetCurrentAnimatorStateInfo (1).IsName (anmName);
	}

	/*public void setAnim(int anmNum){
		switch(anmNum){
			case 0://default
				if(isMoving) anmNum = 1;
				setAnim("default");
				break;
			case 1://run(start)
				setAnim("runStart");
				break;
			case 2://run(continue)
				setAnim("run");
				break;
			case 3://jump1 (clasic jump)
				setAnim("jump");
				break;
		}
	}*/

	void OnTriggerEnter(Collider collis){
		try{
			if (collis.gameObject.layer != scr_gameInit.lyr_def)
			if (collis.gameObject.layer == scr_gameInit.lyr_enemy || collis.gameObject.layer == scr_gameInit.lyr_obj) {
				if(transform.position.y < collis.GetComponent<paramObj>().bCenterY()) collis.gameObject.SendMessage ("OnTouch", 2); else collis.gameObject.SendMessage ("OnTouch", 3);
			}
		} catch(Exception e){

		}
	}

	void FixedUpdate()
	{
		if(scr_gameInit.globalValues.isFocused){
			float h;
			float v;

			if (Physics.Raycast(new Vector3(transform.position.x, transform.position.y+1, transform.position.z), Vector3.down, out hit, 2))
			{
				/*float maxRotation = 45;
				Quaternion targetRotation = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
				transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 50);*/
				isGrounded = true;
			} else isGrounded = false;
			
			#if UNITY_EDITOR
				h = Input.GetAxisRaw("Horizontal");
				v = Input.GetAxisRaw("Vertical");
			#else
				h = UnityEngine.N3DS.GamePad.CirclePad.x;
				v = UnityEngine.N3DS.GamePad.CirclePad.y;
			#endif
			if (isBlocked) {
				h = 0;
				v = 0;
				velocity = 0;
				jumpAct = 3;
			}
			if(h!=0 || v!=0){
				float tmp_walkRotation = 0;
				if(transform.rotation.y < 179 && transform.rotation.y > -179) {
					// adjust angle
					if (tmp_walkRotation < -90) {
						tmp_walkRotation += 360;
					} else if (tmp_walkRotation > 90) {
						tmp_walkRotation -= 360;
					}
					
					walkRotation += tmp_walkRotation / 76;
					tmp_walkRotation = Mathf.Atan2(h, v) * Mathf.Rad2Deg;
				} else {
					tmp_walkRotation = 0; 
				}
				transform.rotation = Quaternion.Euler(transform.eulerAngles.x, tmp_walkRotation+walkRotation+MarioCam.marioCamera.gameObject.transform.eulerAngles.y, transform.eulerAngles.z);
				if (!isMoving) if(jumpAct==0){
					if(currentMoveSpeed>0f) setAnim("run", 0.1f); else setAnim("runStart", 0.05f);
				}
				if(currentMoveSpeed<moveSpeed) currentMoveSpeed+=0.1f;
				if(h < 0f) h = h* -1;
				if (v < 0f)	v = v * -1;
				isMoving = true;
			}
			else { if (isMoving) { Debug.Log ("tru"); 
					setAnim ("default");
					isMoving = false;
				}
			} // if h/v changes, he is moving. (for now)

			// Jump if the jump button is pressed and the object is grounded
			if(isGrounded) {
				groundedPosition = transform.position.y;
				#if UNITY_EDITOR
				if(!hasJumped) if (Input.GetKey(KeyCode.Space)) jumpAct = 1;
				#else
				if(!hasJumped) if (UnityEngine.N3DS.GamePad.GetButtonHold(N3dsButton.A) || UnityEngine.N3DS.GamePad.GetButtonHold(N3dsButton.B)) jumpAct = 1;
				#endif
			}
			
			switch(jumpAct){//case 0 means not jumping at all
			case 0:
				#if UNITY_EDITOR
					if (hasJumped)
					if (!Input.GetKey (KeyCode.Space))
					hasJumped = false;
				#else
					if(hasJumped) 
					if(!UnityEngine.N3DS.GamePad.GetButtonHold(N3dsButton.A) && !UnityEngine.N3DS.GamePad.GetButtonHold(N3dsButton.B)) 
					hasJumped = false;
				#endif
				velocity = 0;
				break;
			case 1://jump up
				hasJumped = true;
				jumpedTime++;
				if (jumpedTime == 1)
					velocity = 0.7f;
				if (jumpedTime == 3)
					setAnim ("jump");
				velocity += jumpForce;
				if ((jumpedTime > maxJump && (!UnityEngine.N3DS.GamePad.GetButtonHold (N3dsButton.A) && !Input.GetKey (KeyCode.Space) && !UnityEngine.N3DS.GamePad.GetButtonHold (N3dsButton.B)))
				    || jumpedTime > maxJump * 2) {
					jumpAct = 2;
					jumpedHeight = transform.position.y;
					fvar0 = jumpedTime;
					jumpedTime = 0;
				}
				break;
			case 2://default
				jumpedTime++;
				velocity += 0.1f;
				if (jumpedTime > 6 + fvar0 * 0.4f) {
					jumpAct = 3;
				}
				break;
			case 3://fall
				if (isGrounded) {
					jumpAct = 0; 
					jumpedTime = 0; 
					if (isMoving) {
						setAnim ("run", 0.1f);
					} else
						setAnim ("default", 0.1f);
				} else {
					velocity = -0.7f;
				}
				break;
			}
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
			if(isHacking){
				if(hasCaptured){
					if (UnityEngine.N3DS.GamePad.GetButtonHold (N3dsButton.L) || UnityEngine.N3DS.GamePad.GetButtonHold (N3dsButton.R)
					    || Input.GetKey (KeyCode.Y) || plsUnhack) {
						plsUnhack = false;
						transform.GetChild (2).gameObject.SetActive (false);//hair
						transform.GetChild (1).gameObject.SetActive(true);//cap
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
						if(cappy.capturedObject.transform.GetChild(0).name == "Mustache") cappy.capturedObject.transform.GetChild (0).gameObject.SetActive (false); //if mustache, place it at index 0
					}
				} else{ 
					if (!isCapturing) {
						isCapturing = true;
						isBlocked = true;
						setAnim ("captureFly");
						jumpAct = 1;
					} else {
						if (anim.GetCurrentAnimatorStateInfo(2).IsName("captureFly")) {
							transform.position = Vector3.MoveTowards (transform.position, cappy.capturedObject.transform.position, 0.3f);
						} else {
							isCapturing = false;
							if(!isBlockBlocked) isBlocked = false;
							hasCaptured = true;
							transform.position = cappy.capturedObject.transform.position;
							cappy.capturedObject.SendMessage ("setState", 6);
							for (int i = 0; i <= 7; i++) {
								transform.GetChild (i).gameObject.SetActive (false);
							}
						}
					}
				}
			} else {
				if(hasCaptured == true){
					hasCaptured = false;
					cappy.SetState(2);
					for(int i = 0; i<=7; i++){
						transform.GetChild (i).gameObject.SetActive(true);
					}
					transform.GetChild (2).gameObject.SetActive(true);//hair
					transform.GetChild (1).gameObject.SetActive(false);//cap
					//transform.GetChild (1).gameObject.SetActive(false);//handL?
				}
			}

			if(!isMoving && currentMoveSpeed>0) currentMoveSpeed-=0.2f;
			
			// Calculate the movement vector based on the input and current speed
			Vector3 movementVector = new Vector3(0f, 0f, h+v) * currentMoveSpeed * Time.deltaTime;

			// Move the rigidbody to the target position using rb.MovePosition
			rb.MovePosition(transform.position + (transform.rotation * Vector3.forward) * movementVector.magnitude + new Vector3(0, velocity *(Time.deltaTime*6), 0));//pos
		}
	}
}