using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_behaviorFrog : MonoBehaviour {

	private string capMount = "Armature/nw4f_root/AllRoot/JointRoot/Spine/Head";

	Animator anim;
	int currentState = 0;

	public void setAnim(string animName, float transitionTime = 0.25f) {
		anim.CrossFade (animName, transitionTime);
	}
	public bool isAnim(string anmName) {
		return anim.GetCurrentAnimatorStateInfo (0).IsName (anmName);
	}
	void Start () {
		anim = GetComponent<Animator> ();
	}

	void setEye(int typeEye){
		switch (typeEye) {
		case 0: 
			transform.GetChild (1).GetChild (2).gameObject.SetActive (false);
			transform.GetChild (1).GetChild (3).gameObject.SetActive (false);
			transform.GetChild (1).GetChild (4).gameObject.SetActive (true);
			break;
		case 1: 
			transform.GetChild (1).GetChild (2).gameObject.SetActive (false);
			transform.GetChild (1).GetChild (3).gameObject.SetActive (true);
			transform.GetChild (1).GetChild (4).gameObject.SetActive (false);
			break;
		case 2: 
			transform.GetChild (1).GetChild (2).gameObject.SetActive (true);
			transform.GetChild (1).GetChild (3).gameObject.SetActive (false);
			transform.GetChild (1).GetChild (4).gameObject.SetActive (false);
			break;
		}
	}
	void setEyeTexture(int numEye){
		transform.GetChild(1).GetChild (5).GetComponent<SkinnedMeshRenderer> ().material = Resources.Load<Material> ("Objects/objFrog/eye."+numEye);
	}
	public void OnTouch(int num){
		if (num == 3)
			anim.Play ("reaction");
	}
	public void OnCapTrigger(){
		scr_main.s.capMountPoint = capMount;
	}
	public void OnCapHacked(){
		gameObject.tag = "captureMe";
		MarioController.s.cappy.SetTransformOffset (1.15f, new Vector3(0, 0, -0.1f), new Vector3(0, -119.16f, 90));
	}
	public void SetState(int num){
		currentState = num;
		switch (num) {
		case 6:
			setAnim ("hackStart", 0.1f);
			setEyeTexture (1);
			MarioController.s.SetSpeed (3, 4);
			GetComponent<Collider> ().enabled = false; //REMOVE
			break;
		case 7:
			setAnim ("swoonStartLand", 0.1f);
			setEyeTexture (0);
			GetComponent<Collider> ().enabled = true; //REMOVE
			break;;
		}
	}

	void Update(){
		switch (currentState) {
		case 6:
			transform.position = MarioController.s.transform.position;
			transform.rotation = MarioController.s.transform.rotation;
			break;
		case 7:
			if(isAnim("default")) SetState(0);
			break;
		}
	}
}
