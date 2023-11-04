using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_behaviorCapSwitch : MonoBehaviour{
	
	private string capMount = "Armature/AllRoot/JointRoot/Swicth/Hat";
	public bool isForward = true;
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
		if(isForward)
			anim.Play ("hitFront");
		else
			anim.Play ("hitBack");
		this.enabled = true;
	}
	void Update(){
		if(anim.GetCurrentAnimatorStateInfo (0).IsName("hitBack2") || anim.GetCurrentAnimatorStateInfo (0).IsName("hitFront2")) MarioController.marioObject.cappy.SetState (2);
	}
}
