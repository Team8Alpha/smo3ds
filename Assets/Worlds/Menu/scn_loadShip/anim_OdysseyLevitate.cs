using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class anim_OdysseyLevitate : MonoBehaviour {

	bool isExiting = false;
	bool hasStarted= false;
	List<GameObject> rootObjects = new List<GameObject>();

	void Start(){
		scr_main._f.SetFocus(false);
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
				scr_loadScene._f.StartScene (scr_loadScene._f.nextScene, 3);
			}
			if (transform.position.x > -100) {
				scr_manAudio._f.FadeBGM(1, 0);
				scr_fadefull._f.Run (false, 0, 0.09f, true, true, true);
				isExiting = true;
			}
		} else if (scr_fadefull._f.isDone) {
			scr_loadScene._f.SetSceneActive ();
			foreach (GameObject _obj in rootObjects)
				if(_obj != null && _obj.name != "objGlobal(Clone)" && _obj.name != "camLoadShip")
					Destroy(_obj);

			scr_main._f.SetFocus(true);
			Destroy(transform.parent.gameObject);
		}
	}
}
