using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_levelInOdyssey : MonoBehaviour {

	void Update () {
		if (MarioController.marioObject != null) {
			Destroy (transform.GetChild (0).gameObject); //destroy child
			Destroy(gameObject);
		}
	}
}
