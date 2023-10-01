using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class anim_OdysseyLevitate : MonoBehaviour {

	bool hasLoaded = false;
	// Update is called once per frame
	void Update () {
		transform.Translate (0, -0.2f *Time.unscaledTime, 0);
		Debug.Log (transform.position);
		if (transform.position.x > -400 && !hasLoaded) {
			hasLoaded = true;
			SceneManager.LoadScene (scr_loadScene._f.nextScene, LoadSceneMode.Additive);
		}
		if (transform.position.x > -100) {
			scr_gameInit.globalValues.focusOn ();
			var gos = GameObject.FindGameObjectsWithTag ("loading");
			foreach (GameObject go in gos)
				Destroy (go);
		}
	}
	//public IEnumerator WaitKill(){
		//for fading out, not implemented yet.
	//}
}
