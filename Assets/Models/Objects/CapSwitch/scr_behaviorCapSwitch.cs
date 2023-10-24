using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_behaviorCapSwitch : MonoBehaviour{
	
	private string capMount = "Armature/AllRoot/JointRoot/Swicth/Hat";

	private Animator anim;

	void Start(){
		anim = GetComponent<Animator> ();
		this.enabled = false;
	}

	public void OnCapture(){
		scr_main._f.capMountPoint = capMount;
	}
	public void OnCaptured(){
		MarioController.marioObject.cappy.headHeight = 0f;
		MarioController.marioObject.cappy.hackScale = 2f;
		MarioController.marioObject.cappy.hackRot = new Vector3 (0, 0, 90);
		if (MarioController.marioObject.cappy.transform.position.z > transform.position.z)
			anim.Play ("hitFront");
		else
			anim.Play ("hitBack");
		this.enabled = true;
	}
}
