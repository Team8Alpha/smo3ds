using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_behaviorCapDoor : MonoBehaviour {

	private Animator anim;
	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator>();
		this.enabled = false;
	}
	public void OnTouch(int numType){
		if (numType == 1) {
			anim.Play("open");
			gameObject.GetComponents<Collider>()[1].enabled = false;
		}
	}
}
