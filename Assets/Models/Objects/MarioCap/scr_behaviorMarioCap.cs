using System;
using UnityEngine;

public class scr_behaviorMarioCap : MonoBehaviour {
	
	private Animator anim;
	private Transform transformMario;
	private Transform armature;
	public int currentState = 1;
	public bool activated = true;
	private float fvar0 = 0;//another var used by states.
	private float offsetYthrow = 0.6f;
	public float headHeight = 0; //height of captured enemy
	private Vector3 mPosTmp;
	public bool isThrown = false;
	int timeStaySpin = 1;
	private AudioSource sndSrc;
	public GameObject capturedObject;
	public GameObject mountPoint = null;
	private int tchCount = 0;
	public float hackScale = 0;
	public Vector3 hackPos = Vector3.zero;
	public Vector3 hackRot = Vector3.zero;
	private bool isColliding = false;
	private bool isHacking = false;
	private Vector3 tmp_pos;
	paramObj paramCaptured;

	string prefixSND = "Sound/Entity/Cappy/";
	AudioClip snd_capHackStart;
	AudioClip snd_capSpin;


	void toggleCollision (bool state){
		gameObject.GetComponents<Collider> ()[1].enabled = state;
		gameObject.GetComponents<Collider> ()[0].enabled = state;
	}

	public void SetState(int state){
		activated = true;
		currentState = state;
		gameObject.SetActive (true);
		switch(state){
		case 0:
			sndSrc.clip = snd_capSpin;
			sndSrc.loop = true;
			sndSrc.Play ();
			isThrown = true;
			MarioController.marioObject.anim.Play ("spinCapStart");
			anim.Play ("default");
            transformMario.GetChild (2).gameObject.SetActive (true);//hair
			transformMario.GetChild (1).gameObject.SetActive (false);//cap
			MarioController.marioObject.SetHand(0, false);//handRball
			MarioController.marioObject.SetHand(1, true);//handRflat
			tmp_pos = transformMario.position;
			transform.rotation = transformMario.rotation;
			transform.position = new Vector3 (tmp_pos.x, tmp_pos.y + offsetYthrow, tmp_pos.z);
			if (MarioController.myState == MarioState.Jumping) {
				currentState = -1;
				anim.Play ("throwJump");
				MarioController.marioObject.anim.Play ("spinCapJumpStart");
				offsetYthrow = 1;
			}
			toggleCollision (true);
			break;
		case 1:
			MarioController.marioObject.SetHand(0, true);//handRball
			MarioController.marioObject.SetHand(1, false);//handRflat
			fvar0 = 0;
			//if (MarioController.marioObject.isMoving)
				//MarioController.marioObject.SetAnim ("run");
			anim.Play ("stay");
			armature.gameObject.transform.eulerAngles = Vector3.zero;
			transform.GetChild (1).gameObject.transform.eulerAngles = Vector3.zero;
			break;
		case 2:
			isHacking = false;
			hackScale = 0;
			transform.localScale = new Vector3 (1,1,1);
			anim.Play ("default");
			break;
		case 3:
			transformMario.GetChild (2).gameObject.SetActive (false);//hair
			transformMario.GetChild (1).gameObject.SetActive (true);//cap
			hackScale = 0;
			transform.localScale = new Vector3 (1,1,1);
			sndSrc.loop = false;
			isThrown = false;
			//transformMario.Find("Armature/nw4f_root/AllRoot/JointRoot/Spine1/Spine2/MarioHead/Cap 1/cappyEyes").gameObject.SetActive (true);//cappyeyes
			gameObject.SetActive(false);
			break;
		case 4:
			toggleCollision (false);
			if (MarioController.marioObject.isHacking) {
				anim.Play ("capture");
				sndSrc.Stop ();
				sndSrc.loop = false;
				sndSrc.clip = snd_capHackStart;
				sndSrc.Play ();
			}
			transform.rotation = Quaternion.Euler (0, 0, 0);
			MarioController.marioObject.SetHand(0, true);//handRball
			MarioController.marioObject.SetHand(1, false);//handRflat
			isThrown = false;
			break;
		case 5:
			toggleCollision (false);
			transformMario.rotation = capturedObject.transform.rotation;
			MarioController.marioObject.SetHand(0, true);//handRball
			MarioController.marioObject.SetHand(1, false);//handRflat
			fvar0 = 0;
			anim.Play ("hookStart");
			break;
		}
	}

	public void setHackData(float scale, Vector3 pos, Vector3 rot){
		hackScale = scale;
		hackPos = pos;
		hackRot = rot;
	}

	void OnTriggerEnter(Collider collis){
		if (collis.gameObject.layer != scr_main.lyr_player)
			isColliding = true;
		//else if(currentState == 1)
		//	MarioController.marioObject.SetState (MarioState.Jumping); //problems with collision
		if (collis.gameObject.GetComponent<paramObj> () != null)
			paramCaptured = collis.gameObject.GetComponent<paramObj> ();
	}
	void OnTriggerStay(Collider collis){
		if (paramCaptured == null)
			return;
		if (collis.gameObject.layer != scr_main.lyr_player) {
			if (collis.gameObject.layer != scr_main.lyr_def) {
				try {
					if (tchCount == 0) {

						tchCount = 50;

						if (collis.gameObject.layer == scr_main.lyr_enemy) {
							if(paramCaptured.isTouch) collis.gameObject.SendMessage ("OnTouch", 1);
							if(paramCaptured.isCapture) collis.gameObject.SendMessage ("OnCapture"); //send OnCapture event to object
							else return;
							mountPoint = collis.gameObject.transform.Find (scr_main._f.capMountPoint).gameObject;
							if (mountPoint == null) {//if not set by object, or set wrongly, dont capture.
								scr_main._f.SetCMD ("NO CPMNT AT " + scr_main._f.capMountPoint + " IN " + collis.gameObject);
								capturedObject = null;
							} else if (!isHacking) {
								scr_main._f.SetCMD ("crappy hav frund " + collis.gameObject.name);
                                /*if (collis.gameObject.GetComponentInChildren<scr_behaviorGoomba>() != null && collis.gameObject.GetComponentInChildren<scr_behaviorGoomba>().isTop && collis.gameObject.GetComponent<scr_behaviorGoomba>().stackAmount > 0)
                                    capturedObject = GameObject.Find("goombaOnTop");
                                else*/
                                    capturedObject = collis.gameObject;

								if(collis.gameObject.GetComponent<Collider>() != null)
									collis.gameObject.GetComponent<Collider>().enabled = false;
								if(collis.gameObject.GetComponent<Rigidbody>() != null)
									collis.gameObject.GetComponent<Rigidbody>().useGravity = false;
								toggleCollision(false);
								capturedObject.SendMessage ("OnCaptured"); //send OnCaptured event to object
								if(paramCaptured.isHack) MarioController.marioObject.isHacking = true;
								else anim.Play("hookStart");
								SetState (4);
								armature.eulerAngles = Vector3.zero;
								transform.GetChild (1).gameObject.transform.eulerAngles = Vector3.zero;
								var Mustache = capturedObject.transform.GetChild(0);
								if(Mustache.name == "Mustache" || Mustache.name == "Mustache__HairMT") Mustache.gameObject.SetActive (true); //if mustache, place it at index 0
								scr_main._f.SetCMD ("CAPMOUNT AT " + mountPoint.name);
								isHacking = true;
							}
						} else if (collis.gameObject.layer == scr_main.lyr_obj) {
							if(paramCaptured.isTouch) collis.gameObject.SendMessage ("OnTouch", 1);
							if(paramCaptured.isCapture) collis.gameObject.SendMessage ("OnCapture", 1);
							capturedObject = collis.gameObject;
							scr_main._f.SetCMD ("crappy hav pfund" + collis.gameObject + collis.gameObject.layer.ToString ());
						}
					} else {
						tchCount--;
					}
				} catch (Exception e){
					scr_main._f.SetCMD ("ERROR at CapTrigger: " + e);
				}
			}
		}
	}
	void OnTriggerExit(Collider collis){
		tchCount = 0;
		isColliding = false;
	}
	
	// Use this for initialization
	void Awake () {
		MarioController.marioObject.cappy = this;
		anim = GetComponent<Animator>();
		tmp_pos = transform.position;
		sndSrc = GetComponent<AudioSource> ();
		transformMario = MarioController.marioObject.transform;
		armature = transform.GetChild(0);

		snd_capHackStart = scr_manageAudio._f.GetClip(prefixSND+"snd_capHackStart");
		snd_capSpin = snd_capSpin = scr_manageAudio._f.GetClip (prefixSND + "snd_capSpin");
	}

	// Update is called once per frame
	void Update () {
		if(scr_main._f.isFocused) if(activated){
			switch(currentState){
			case -1://jump throw start
				tmp_pos = transformMario.position;
				transform.rotation = transformMario.rotation;
				transform.position = new Vector3 (tmp_pos.x, tmp_pos.y + 1.1f, tmp_pos.z);
				if (anim.GetCurrentAnimatorStateInfo (0).normalizedTime > 1) {
					currentState++;				
					mPosTmp = transformMario.position;
					tmp_pos = transform.position;
				}
				break;
			case 0://throw
				if (Vector3.Distance (transform.position, tmp_pos) > 8) {
					SetState (1);
					transform.Translate (new Vector3 (0, 0, -0.6f));
					break;
				}
				transform.Translate (new Vector3 (0, 0, 1f));
				armature.Rotate (0, 40, 0); 
				break;
			case 1://spin after throw
				armature.Rotate (0, 50, 0); 
				fvar0 += 0.1f;
				if (fvar0 > timeStaySpin && !UnityEngine.N3DS.GamePad.GetButtonHold (N3dsButton.Y) && !UnityEngine.N3DS.GamePad.GetButtonHold (N3dsButton.X) && !Input.GetKey (KeyCode.LeftShift))
					SetState (2);
				break;
			case 2://go back
				armature.Rotate (0, 50, 0);
				mPosTmp = transformMario.position;
				transform.position = Vector3.MoveTowards (transform.position, new Vector3 (mPosTmp.x, mPosTmp.y + offsetYthrow, mPosTmp.z), 50 * Time.deltaTime);
				if (transform.position == new Vector3 (mPosTmp.x, mPosTmp.y + offsetYthrow, mPosTmp.z)) {
					activated = false;
					SetState (3);
				}
				break;
			case 3: //nothing.
				break;
			case 4: //captured an enemy.
				transform.rotation = mountPoint.transform.rotation;
				transform.position = mountPoint.transform.position;
				transform.GetChild(0).localEulerAngles = hackRot;
				transform.GetChild(0).localPosition = hackPos;
				if(hackScale != 0) transform.localScale = new Vector3 (hackScale, hackScale, hackScale); else transform.localScale = mountPoint.transform.localScale;
				break;
			case 5: //rotating hook, hanger.
				fvar0 += 0.1f;
				if (fvar0 > timeStaySpin && !UnityEngine.N3DS.GamePad.GetButtonHold (N3dsButton.Y)
				    && !UnityEngine.N3DS.GamePad.GetButtonHold (N3dsButton.X)
				    && !Input.GetKey (KeyCode.LeftShift))
					SetState (2);
				break;
			}
		}
	}
}
