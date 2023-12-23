using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_levelInOdyssey : MonoBehaviour {

	void LateUpdate () {
		if (MarioController.s != null) {
			MarioCam.s.gameObject.SetActive (false);
			this.enabled = false;
		}
	}
}
