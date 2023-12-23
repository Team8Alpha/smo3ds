using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class anim_OdysseyLevitate : MonoBehaviour {

	bool isExiting = false;
	bool hasStarted= false;
	List<GameObject> rootObjects = new List<GameObject>();

	void Start(){
		scr_main.s.SetFocus(false);
		Application.backgroundLoadingPriority = ThreadPriority.BelowNormal;
		for (int i = 0; i < SceneManager.sceneCount; i++)
		{
			rootObjects.AddRange(SceneManager.GetSceneAt(i).GetRootGameObjects());
		}

	}
	// Update is called once per frame
	void Update () {
		if (!isExiting) {
			transform.Translate (0, -0.17f * Time.unscaledTime, 0); 
			if (!hasStarted) {
				hasStarted = true;
				scr_loadScene.s.StartScene (scr_loadScene.s.nextScene, 3);
			}
			if (transform.position.x > -100) {
				scr_manAudio.s.FadeBGM(1, 0);
				scr_fadefull.s.Run (false, 0, 0.09f, true, true, true);
				isExiting = true;
			}
		} else if (scr_fadefull.s.isDone) {
			scr_loadScene.s.SetSceneActive ();
			foreach (GameObject _obj in rootObjects)
				if(_obj != null && _obj.name != "objGlobal(Clone)" && _obj.name != "camLoadShip")
					Destroy(_obj);

			scr_main.s.SetFocus(true);
			Destroy(transform.parent.gameObject);
		}
	}
}
