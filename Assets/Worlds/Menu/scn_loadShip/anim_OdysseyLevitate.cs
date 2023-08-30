﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class anim_OdysseyLevitate : MonoBehaviour {
	
	// Update is called once per frame
	void Update () {
		transform.Translate (0, -6, 0);
		if (transform.position.x > -100 && transform.position.x < -95) {
			SceneManager.LoadScene (scr_loadScene._f.nextScene, LoadSceneMode.Additive);
			scr_gameInit.globalValues.focusOn ();
			var gos = GameObject.FindGameObjectsWithTag("loading");
			foreach(GameObject go in gos)
				Destroy(go);
		}
	}
	//public IEnumerator WaitKill(){
		//for fading out, not implemented yet.
	//}
}
