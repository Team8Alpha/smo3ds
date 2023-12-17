using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class scr_behaviorStake : MonoBehaviour {

	private string capMount = "Armature/AllRoot/Center/Stake/JointRoot/Bend0/Bend1/Bend2/CapPoint";

	private float bentH; private float bentV;
	private Transform boneBend0;
	private Transform boneBend1;
	private Transform boneBend2;
	private float bentAmountMax = 20;
	private float bentAmount;
	private Transform capBone;
	private Animator anim;

	void Start(){
		anim = GetComponent<Animator> ();
		boneBend0 = transform.Find ("Armature/AllRoot/Center/Stake/JointRoot/Bend0");
		boneBend1 = boneBend0.GetChild (0);
		boneBend2 = boneBend1.GetChild (0);
		capBone  = transform.Find ("Armature/AllRoot/Center/Stake/JointRoot/Bend0/Bend1/Bend2/CapPoint");
		this.enabled = false;
	}

	public void OnCapTrigger(){
		scr_main._f.capMountPoint = capMount;
	}
	public void OnCapHacked(){
		gameObject.tag = "captureMe"; //only used for its-own-script identification, since i modified some stuff.
									  //CAP! MarioController.marioObject.cappy.headHeight = 0f;
									  //CAP! MarioController.marioObject.isHacking = false;
									  //CAP! MarioController.marioObject.cappy.hackScale = 2f;
		anim.Play ("pull");
		bentH = scr_manageInput.AxisDir(-1).x;
		bentV = scr_manageInput.AxisDir(-1).y;
		this.enabled = true;
	}

	void Update () {
		if (scr_main._f.isFocused) {
			if (tag == "captureMe") {
				if (bentAmount < bentAmountMax && bentAmount != -0.1f)
					bentAmount += 0.5f;
				else if (bentAmount != -0.1f) {
					GetComponent<Collider> ().enabled = false;
					anim.Play ("pullOut");
					//CAP! MarioController.marioObject.cappy.SetState (2);
					bentAmount = -0.1f;
				} else if (anim.GetBool ("isDead")) {
					Destroy (gameObject);
				} else {
					//fly away
				}
				Vector3 tmp_001 = new Vector3 (0, bentV * bentAmount, bentH * bentAmount);
				boneBend0.transform.localEulerAngles = tmp_001;
				boneBend1.transform.localEulerAngles = tmp_001;
				boneBend2.transform.localEulerAngles = tmp_001;

				capBone.localRotation = Quaternion.LookRotation (Vector3.up, capBone.up);
				//scr_main.DPrint (capBone.localEulerAngles);
				capBone.localEulerAngles = new Vector3 (-capBone.localEulerAngles.y, 0, 90);
			}
		}
	}
}
