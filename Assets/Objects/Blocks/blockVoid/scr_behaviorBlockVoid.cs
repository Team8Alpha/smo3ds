using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_behaviorBlockVoid : MonoBehaviour {

	// Use this for initialization
	void Start () {
		gameObject.GetComponent<Animator>().Play("up");
		this.enabled = false;
	}
}
