using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_behaviorCapDoor : MonoBehaviour {

	public int currentState = 0;
	private Animator anim;
	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator>();
	}
	public void OnTouch(int numType){
		if (numType == 1) {
			anim.Play("open");
			transform.GetChild (5).gameObject.SetActive (false);
		}
	}
}
