using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class scr_behaviorMarioCap : MonoBehaviour {
	
	private Animator anim;
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
	private Collider[] collider;
	private Transform transformMario;
	private Transform armature;
	
	private Vector3 tmp_pos;

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
			sndSrc.clip = Resources.Load<AudioClip> ("Audio/Sounds/Cappy/snd_capSpin");
			sndSrc.loop = true;
			sndSrc.Play ();
			isThrown = true;
			MarioController.marioObject.anim.Play ("spinCapStart");
			anim.Play ("default");
			transformMario.GetChild (2).gameObject.SetActive (true);//hair
			transformMario.GetChild (1).gameObject.SetActive (false);//cap
			MarioController.marioObject.SetHand(0, false);//handRball
			MarioController.marioObject.SetHand(1, true);//handRflat
			//transformMario.Find("Armature/nw4f_root/AllRoot/JointRoot/Spine1/Spine2/MarioHead/Cap 1/cappyEyes").gameObject.SetActive (false);//cappyeyes
			tmp_pos = transformMario.position;
			transform.rotation = transformMario.rotation;
			transform.position = new Vector3 (tmp_pos.x, tmp_pos.y + offsetYthrow, tmp_pos.z);
			if (MarioController.marioObject.jumpAct != 0) {
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
			if (MarioController.marioObject.isMoving)
				MarioController.marioObject.SetAnim ("run");
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
				sndSrc.clip = Resources.Load<AudioClip> ("Audio/Sounds/Cappy/snd_capTure");
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
		if (collis.gameObject.layer != scr_gameInit.lyr_player) isColliding = true;
	}
	void OnTriggerStay(Collider collis){
		if (collis.gameObject.layer != scr_gameInit.lyr_player) {
			if (collis.gameObject.layer != scr_gameInit.lyr_def) {
				try {
					if (tchCount == 0) {

						tchCount = 50;

						if (collis.gameObject.layer == scr_gameInit.lyr_enemy) {
							collis.gameObject.SendMessage ("OnTouch", 1);
							collis.gameObject.SendMessage ("OnCapture"); //send OnCapture event to object
							mountPoint = collis.gameObject.transform.Find (scr_gameInit.globalValues.capMountPoint).gameObject;
							if (mountPoint == null) {//if not set by object, or set wrongly, dont capture.
								scr_gameInit.globalValues.capMountPoint = "missingno";
								Debug.Log ("NO CPMNT AT " + scr_gameInit.globalValues.capMountPoint + " IN " + collis.gameObject);
								capturedObject = null;
							} else if (!isHacking) {
								Debug.Log ("crappy hav frund " + collis.gameObject.name);
								capturedObject = collis.gameObject;
								if(collis.gameObject.GetComponent<Collider>() != null)
									collis.gameObject.GetComponent<Collider>().enabled = false;
								if(collis.gameObject.GetComponent<Rigidbody>() != null)
									collis.gameObject.GetComponent<Rigidbody>().useGravity = false;
								toggleCollision(false);
								MarioController.marioObject.isHacking = true;
								capturedObject.SendMessage ("OnCaptured"); //send OnCaptured event to object
								SetState (4);
								armature.eulerAngles = Vector3.zero;
								transform.GetChild (1).gameObject.transform.eulerAngles = Vector3.zero;
								var Mustache = capturedObject.transform.GetChild(0);
								if(Mustache.name == "Mustache" || Mustache.name == "Mustache__HairMT") Mustache.gameObject.SetActive (true); //if mustache, place it at index 0
								Debug.Log ("CAPMOUNT AT " + mountPoint.name);
								isHacking = true;
							}
						} else if (collis.gameObject.layer == scr_gameInit.lyr_obj) {
							collis.gameObject.SendMessage ("OnTouch", 1);
							capturedObject = collis.gameObject;
							Debug.Log ("crappy hav pfund" + collis.gameObject + collis.gameObject.layer.ToString ());
						}
					} else {
						tchCount--;
					}
				} catch (Exception e){
					Debug.Log ("ERROR at CapTrigger: " + e);
				}
			}
		}
	}
	void OnTriggerExit(Collider collis){
		tchCount = 0;
		isColliding = false;
	}
	
	// Use this for initialization
	void Start () {
		gameObject.SetActive(false);
		MarioController.marioObject.cappy = this;
		anim = GetComponent<Animator>();
		tmp_pos = transform.position;
		sndSrc = GetComponent<AudioSource> ();
		collider = gameObject.GetComponents<Collider> ();
		transformMario = MarioController.marioObject.transform;
		armature = transform.GetChild(0);
	}

	// Update is called once per frame
	void Update () {
		if(scr_gameInit.globalValues.isFocused) if(activated){
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
				if (Vector3.Distance (transform.position, tmp_pos) > 6) {
					SetState (1);
					transform.Translate (new Vector3 (0, 0, -0.6f));
					break;
				}
				transform.Translate (new Vector3 (0, 0, 0.8f));
				armature.Rotate (0, 30, 0); 
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
