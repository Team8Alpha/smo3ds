using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class scr_loadScene : MonoBehaviour {
	
	[HideInInspector] public static scr_loadScene _f;
	[HideInInspector] public string nextScene = "scn_menuTitle";
	[HideInInspector] public bool isDone = false;
	AsyncOperation loadOP;

	void Start(){ _f = this; }
	public void StartScene(string sceneName, int transition = 0){
		isDone = false;
		string currentscn = SceneManager.GetActiveScene ().name;
		scr_main._f.hasLevelLoaded = false;
		nextScene = sceneName;
		scr_main._f.dbg_enemyCount = 0;
		GetComponent<AudioListener> ().enabled = true;
//		if (MarioController.marioObject != null)
//			MarioController.marioObject.gameObject.GetComponent<AudioListener> ().enabled = false;
		scr_main._f.SetCMD ("nSCN: " + nextScene);
		switch (transition) {
		case 0: //direct, no transition
			SceneManager.LoadScene (sceneName, LoadSceneMode.Additive);
			SceneManager.UnloadSceneAsync (currentscn);
			break;
		case 1: //flying ship line
			scr_main._f.focusOff ();
			SceneManager.LoadScene ("scn_loadShip");
			nextScene = sceneName;
				//wip
			break;
		case 2://cap fly transition
			scr_main._f.transform.GetChild (1).GetChild (1).gameObject.SetActive (true);
			break;
		//case 3://basic transition
		//scr_fadefull._f.Run ( );
		//break;
		case 3: 
			StartCoroutine (loadAsync ());
			break;
		}
	}
	IEnumerator loadAsync(){
		loadOP = SceneManager.LoadSceneAsync (nextScene, LoadSceneMode.Additive);
		loadOP.allowSceneActivation = false;
		while (!loadOP.isDone) {
			scr_main._f.SetCMD ("loading: " + (loadOP.progress*100) + "%", false);
			yield return null;
		}
		scr_main._f.SetCMD ("loading: 100%");
		isDone = true;
	}
	public void SetSceneActive(){
		loadOP.allowSceneActivation = true;
	}
}
