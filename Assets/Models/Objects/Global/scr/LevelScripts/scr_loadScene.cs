using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class scr_loadScene : MonoBehaviour {
	
	public static scr_loadScene _f;
	public string nextScene = "scn_menuTitle";
	
	public void loadTransition(string sceneName, int transition = 0){
		scr_gameInit.globalValues.hasLevelLoaded = false;
		nextScene = sceneName;
		scr_gameInit.globalValues.dbg_enemyCount = 0;
		GetComponent<AudioListener> ().enabled = true;
		if (MarioController.marioObject != null)
//			MarioController.marioObject.gameObject.GetComponent<AudioListener> ().enabled = false;
		Debug.Log ("nSCN: "+ nextScene);
		switch (transition) {
		case 0: //direct, no transition
			SceneManager.LoadScene (sceneName);
			break;
		case 1: //flying ship line
			scr_gameInit.globalValues.focusOff ();
			SceneManager.LoadScene ("scn_loadShip");
			nextScene = sceneName;
				//wip
			break;
		case 2://cap fly transition
			scr_gameInit.globalValues.transform.GetChild (3).GetChild (1).gameObject.SetActive (true);
			break;
		//case 3://basic transition
		//scr_fadefull._f.Run ( );
			//break;
		}
	}
	void Start(){ _f = this; }
}
