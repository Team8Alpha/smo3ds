using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_behaviorGoomba : MonoBehaviour {
	//GOOMBA

	public int currentState = 0;//state of the goomba; walking, waiting, attacking, falling...
	private int statesState = 0; //for states in state, like is rotating while walking
	public int stackAmount = 1;//if 1, its just one goomba.
	public bool isMoving = false;//is moving
	public GameObject controller;//not used if isMoving is true
	public int stackedNum = 1;//2 is the first stacked goomba, 3 the second,  and only works with isMoving set to false and controller set to object.
	private bool dead = false;//ded
	private float fvar0 = 0; //var to be used by states
	private float fvar1 = 0; //var to be used by states
	private float fvar2 = 0; //var to be used by states
	private scr_behaviorGoomba[] goombaSt = new scr_behaviorGoomba[20];
	private bool isTop = true; //for stacked goomba stuff
	/*
	0 waiting
	1 walking
	2 stacked goomba
	*/
	public float walkSpeed = 6;
	public float rotationSpeed = 4;
	public Animator anim;

	public void setAnim(string animName, float transitionTime = 0.25f) {
		anim.CrossFade (animName, transitionTime);
	}
	public bool isAnim(string anmName) {
		return anim.GetCurrentAnimatorStateInfo (0).IsName (anmName);
	}

	void setEye(int typeEye){
		switch (typeEye) {
		case 0: //open
			transform.GetChild (4).gameObject.SetActive (false);
			transform.GetChild (5).gameObject.SetActive (false);
			transform.GetChild (6).gameObject.SetActive (true);
			break;
		case 1: //half closed
			transform.GetChild (4).gameObject.SetActive (false);
			transform.GetChild (5).gameObject.SetActive (true);
			transform.GetChild (6).gameObject.SetActive (false);
			break;
		case 2: //closed
			transform.GetChild (4).gameObject.SetActive (true);
			transform.GetChild (5).gameObject.SetActive (false);
			transform.GetChild (6).gameObject.SetActive (false);
			break;
		}
	}

	void setEyeTexture(int numEye){
		transform.GetChild (6).transform.GetChild (0).GetComponent<SkinnedMeshRenderer> ().material = Resources.Load<Material> ("Objects/objGoomba/eye."+numEye);
		transform.GetChild (6).transform.GetChild (1).GetComponent<SkinnedMeshRenderer> ().material = Resources.Load<Material> ("Objects/objGoomba/eye."+numEye);
	}

	void setState(int stateNum){
		currentState = stateNum;
		statesState = 0;
		fvar0 = 0; //var to be used by states
		fvar1 = 0; //var to be used by states
		fvar2 = 0; //var to be used by states
		switch(stateNum){
		case 0:
			setAnim ("wait");
			setEye (0);
			break;
		case 1:
			setAnim ("walk");
			break;
		case 3:
			setAnim ("find");
			setEye (0);
			transform.LookAt (MarioController.marioObject.transform);
			transform.rotation = Quaternion.Euler (0, transform.eulerAngles.y, 0);
			break;
		case 4:
			setAnim ("dash", 1);
			setEye (1);
			break;
		case 6:
			setAnim ("hackStart", 0.1f);
			setEyeTexture (1);
			transform.GetChild (3).gameObject.SetActive (false);
			MarioController.marioObject.SetSpeed (3, 4, 0.7f);
			break;
		case 7:
			setAnim ("hackEnd", 0.1f);
			setEyeTexture (0);
			transform.GetChild (3).gameObject.SetActive (true);
			break;
		}
	}
	
	void walkRotation(float angle){
		transform.Rotate(new Vector3(0, angle, 0));
	}
	
	// Use this for initialization
	void Start () {
		scr_gameInit.globalValues.dbg_enemyCount++;
		anim = GetComponent<Animator> ();
		if(stackAmount>1){
			isTop = false;
			float stackOffY = 3;
			int i = 1;
			for(i=0; i<stackAmount; i++){
				Debug.Log(i+2);
				GameObject goombaStacked = scr_summon.f_summon.s_entity(0, new Vector3(transform.position.x, transform.position.y+(i*stackOffY), transform.position.z), new Vector3(transform.rotation.x, transform.rotation.y, transform.rotation.z));
				goombaSt[i] = goombaStacked.GetComponent<scr_behaviorGoomba>();
				goombaSt[i].isMoving = false;
				goombaSt[i].controller = gameObject;
				goombaSt[i].stackAmount=1;
				goombaSt[i].currentState=2;
				goombaSt[i].stackedNum=i+1;
				if(i!=stackAmount-2) goombaSt[i].isTop = false; else goombaSt[i].isTop = true;
				goombaSt[i].transform.parent = transform;
			}
		}
	}
	
	public void OnTouch(int numType){
		if (currentState != 6 && isTop == true) {
			switch (numType) {
			case 3:
				gameObject.tag = "Untagged";
				setAnim ("pressDown");
				dead = true;
				scr_gameInit.globalValues.dbg_enemyCount--;
				break;
			case 2:
				//kill mario
				Debug.Log("wow");
				break;
			}
		}
	}

	public void OnCapture(){
		scr_gameInit.globalValues.capMountPoint = "Armature/nw4f_root/AllRoot/JointRoot/Head/Cap";
	}
	public void OnCaptured(){
		gameObject.tag = "captureMe";
		MarioController.marioObject.cappy.setHackData (1.525f, new Vector3 (0, 0.5f, 0), new Vector3(-6,0,0));
	}
	
	// Update is called once per frame
	void Update () {
		if(scr_gameInit.globalValues.isFocused){
			if (dead) {
				if (anim.GetBool ("flat")) {
					for (int i = 1; i < 7; i++) {
						transform.GetChild (i).gameObject.SetActive (false);
					}
					transform.GetChild (7).gameObject.SetActive (true);
				}
				if (anim.GetBool ("dead"))
					Destroy (gameObject);
			} else {

				switch (currentState) {
				case 0: //wait
					if (fvar0 == 0) { //init state
						fvar1 = Random.Range (50, 500);
					}
					if (fvar0 != fvar1) { //run state
						fvar0++;
					} else {
						setState (1); //end state
					}
					break;
				case 1: //idling around
					switch (statesState) {
					case 1://walk straight
						if (fvar0 == 0) {
							fvar1 = Random.Range (10, 300); //get random length of path
						}
						if (fvar0 != fvar1) {
							fvar0++; //walk the path
							transform.Translate (new Vector3 (0, 0, walkSpeed * Time.deltaTime));
						} else {
							setState (0);//end the path
						}

						break;
					case 0://rotating with walk
						if (fvar0 == 0) {
							fvar1 = Random.Range (3, 200); //path
							fvar2 = Random.Range (0, 2) * 2 - 1; //rotation
						}
						if (fvar0 != fvar1) {
							fvar0++;
							walkRotation (rotationSpeed * fvar2);
							transform.Translate (new Vector3 (0, 0, walkSpeed * Time.deltaTime));
						} else {
							fvar0 = 0;
							statesState = 1;
						}
						break;
					}
					break;
				case 2: //stacked goomba
					//anim.Play ("wait"); //stacked goombas have been disabled temporary in scr_spn_goomba, because it crashes when the commented line is uncommented.
					if (controller != null) {
						transform.position = new Vector3 (controller.transform.position.x, controller.transform.position.y + ((stackedNum - 1) * 1.4f), controller.transform.position.z);
						transform.rotation = controller.transform.rotation;
					}
					break;
				//case 3: //saw player 
				//	if(isAnim("default")) setState(4);
				//	break;

				case 6: //controller, each enemy has 6 as its controlled state(for organization?)
					transform.position = MarioController.marioObject.transform.position;
					transform.rotation = MarioController.marioObject.transform.rotation;
					break;
				case 7: //confused state after leaving capture
					if(isAnim("default")) setState(0);
					break;
				}
			}
		}
	}
}
