using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_behaviorCheckpoint : MonoBehaviour {

	public int numSpawnPoint = 0; //which spawn point is this checkpoint adressed to.
	private bool wasActivated = false;
	[SerializeField] private Material mat_after; 

	Animator anim;
	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator> ();
		anim.Play ("before");
	}
	void f_take(){
		if (!wasActivated){
			anim.Play("get");
			transform.GetChild(2).gameObject.GetComponent<Renderer>().material = mat_after;
			wasActivated = true;
			scr_gameInit.globalValues.lastCheckpoint = numSpawnPoint;
			scr_manageData._f.Save ();
		}
	}
	void OnTouch(int numType){
		switch (numType) {
		case 1://cap
			f_take();
			break;
		case 2://mar
			f_take ();
			//MarioController.marioObject.setAnim (" ");
			break;
		}
	}
}