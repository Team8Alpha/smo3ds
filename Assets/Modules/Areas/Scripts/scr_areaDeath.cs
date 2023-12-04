using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_areaDeath : MonoBehaviour {

	void Start(){
		this.enabled = false;
	}

	void OnTriggerEnter(Collider col){
		if (col.name == "Mario") {
			//MarioScene._f.PlayerDied(1);
			scr_main.DPrint ("AREA: death");
		}
	}
}
