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

	public void OnTouch(int num){
		if(num == 1) scr_main.s.capMountPoint = capMount;
	}
	public void OnCapHacked(){//CAP!
		//MarioController.s.cappy.headHeight = 0f;
		//MarioController.s.cappy.hackScale = 2f;
		//MarioController.s.cappy.hackRot = new Vector3 (0, 0, 90);
		if(isForward)
			anim.Play ("hitFront");
		else
			anim.Play ("hitBack");
		this.enabled = true;
	}
	void Update(){
		//CAP! if(anim.GetCurrentAnimatorStateInfo (0).IsName("hitBack2") || anim.GetCurrentAnimatorStateInfo (0).IsName("hitFront2")) MarioController.s.cappy.SetState (2);
	}
}
