using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_behaviorGoomba : MonoBehaviour {
	//GOOMBA

	public int currentState = 0;//state of the goomba; walking, waiting, attacking, falling...
	private int statesState = 0; //for states in state, like is rotating while walking
	public int stackAmount = 1;//if 1, its just one goomba.
	public bool isMoving = false;//is moving
	public int stackedNum = 1;//2 is the first stacked goomba, 3 the second,  and only works with isMoving set to false and controller set to object.
	private bool dead = false;//ded
	private float fvar0 = 0; //var to be used by states
	private float fvar1 = 0; //var to be used by states
	private float fvar2 = 0; //var to be used by states
	private int findTimer = 0; // til goomba is turnin over
	private scr_behaviorGoomba[] goombaSt = new scr_behaviorGoomba[20];
	public bool isTop = true; //for stacked goomba stuff
	/*
	0 waiting
	1 walking
	2 stacked goomba
	*/
	public float[] walkSpeed = {6, 12};
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
			findTimer = 0;
			setAnim ("wait");
			break;
		case 4:
			setAnim ("find");
			setEye (0);
			break;
		case 5:
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
		scr_main._f.dbg_enemyCount++;
		anim = GetComponent<Animator> ();
		if(stackAmount>1){
			isTop = false;
			float stackOffY = 2.5f;
			int i = 1;
			for(i=0; i<stackAmount-1; i++){
				GameObject goombaStacked = scr_summon.f_summon.s_entity(0, new Vector3(transform.position.x, transform.position.y+(i+1*stackOffY), transform.position.z), new Vector3(transform.rotation.x, transform.rotation.y, transform.rotation.z));
				goombaSt[i] = goombaStacked.GetComponent<scr_behaviorGoomba>();
				goombaSt[i].isMoving = false;
				goombaSt[i].stackAmount=1;
				goombaSt[i].currentState=2;
				goombaSt[i].stackedNum=i+1;
				if(i!=stackAmount-2) goombaSt[i].isTop = false; else goombaSt[i].isTop = true;
                if (goombaSt[i].isTop)
                    goombaSt[i].name = "goombaOnTop";
                goombaSt[i].transform.parent = transform;
			}
		}
	}

	void RankBelowGoomba(){
		RaycastHit hit;
		if (Physics.Raycast (transform.position, Vector3.down, out hit, 1f)) {
			scr_behaviorGoomba obj = hit.transform.gameObject.GetComponent<scr_behaviorGoomba> ();
			obj.isTop = true;
		} else
			scr_main._f.SetCMD ("single.");
	}
	
	public void OnTouch(int numType){
		if (currentState != 6) {
			switch (numType) {
			case 3:
				if (!isTop)
					break; //spare his life
				gameObject.tag = "Untagged";
				setAnim ("pressDown");
				GetComponent<Collider> ().enabled = false;
				dead = true;
				scr_main._f.dbg_enemyCount--;
				if (isMoving == false)
					RankBelowGoomba ();
				break;
			case 2:
					//kill mario
					scr_main._f.SetCMD("~YOU DIED~");
				break;
			}
		}
	}

	public void OnCapture(){
		scr_main._f.capMountPoint = "Armature/nw4f_root/AllRoot/JointRoot/Head/Cap";
	}
	public void OnCaptured(){
		gameObject.tag = "captureMe";
		MarioController.marioObject.cappy.setHackData (1.525f, new Vector3 (0, 0.5f, 0), new Vector3(-6,0,0));
	}
	public void OnSensorEnter(Collider col){
		if (col.name == "mario" && isMoving && !dead)
			setState (3); 
	}
	public void OnSensorExit(Collider col){
		if (col.name == "mario" && isMoving && !dead)
			setState (0);
	}

	// Update is called once per frame
	void Update () {
		if(scr_main._f.isFocused){
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
							transform.Translate (new Vector3 (0, 0, walkSpeed[0] * Time.deltaTime));
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
							transform.Translate (new Vector3 (0, 0, walkSpeed[0] * Time.deltaTime));
						} else {
							fvar0 = 0;
							statesState = 1;
						}
						break;
					}
					break;
				case 2: //stacked goomba
					break;
				case 3:
					findTimer++;
					if (findTimer >= 20)
						setState (4);
					break;
				case 4: //saw player 
					if (anim.GetCurrentAnimatorStateInfo (0).normalizedTime < 1)
						setState (5);
					break;
				case 5: //ATTACK
    				// Calculate the y rotation
					Vector3 relativePos = MarioController.marioObject.transform.position - transform.position;
					float yRotation = Mathf.Atan2 (relativePos.x, relativePos.z) * Mathf.Rad2Deg;

    				// Rotate the object to face the target
					transform.rotation = Quaternion.Euler (0, yRotation, 0);

    				// Move the object towards the target
					transform.position += transform.forward * walkSpeed[1] * Time.deltaTime;
					break;
				case 6: //controller, each enemy has 6 as its controlled state(for organization?)
					transform.position = MarioController.marioObject.transform.position;
					transform.rotation = MarioController.marioObject.transform.rotation;
					break;
				case 7: //confused state after leaving capture
					if (isAnim ("default"))
						setState (0);
					break;
				}
			}
		}
	}
}
