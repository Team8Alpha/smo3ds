using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class anim_OdysseyLevitate : MonoBehaviour {

	bool hasLoaded = false;
	bool isExiting = false;

	// Update is called once per frame
	void Update () {
		if (!isExiting) {
			transform.Translate (0, -0.17f * Time.unscaledTime, 0);
			if (transform.position.x > -100) {
				hasLoaded = true;
				scr_manageAudio._f.AudioFadeOut (1);
				scr_fadefull._f.Run (false, 0, 0.09f, true, true, true);
				isExiting = true;
			}
		} else if (scr_fadefull._f.isDone) {
			SceneManager.LoadScene (scr_loadScene._f.nextScene, LoadSceneMode.Additive);
			var gos = GameObject.FindGameObjectsWithTag ("loading");
			foreach (GameObject go in gos)
				Destroy (go);
		}
	}
	//public IEnumerator WaitKill(){
		//for fading out, not implemented yet.
	//}
}
