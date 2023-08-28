using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class scr_menuSplashPress : MonoBehaviour {

	[SerializeField] float t_timer = 5;
	private bool t_exiting = false;
	void Confirm(){
		Debug.Log ("Loading...");
		//scr_loadScene._f.loadTransition ("scn_capMain0", 1);
		UnityEngine.SceneManagement.SceneManager.LoadScene("scn_menuTitle");
		scr_manageData._f.Load ();
	}

	void Start(){
		scr_fadefull._f.Run (true, 0, 0.04f);
	}

	void Update () {
		t_timer -= 0.1f;
		if(t_timer<=0){
			if (t_exiting) {
				Confirm();
			} else {
				scr_fadefull._f.Run (false, 0, 0.08f);
				t_timer = 2;
				t_exiting = true;
			}
		}
		#if UNITY_EDITOR
		if(!t_exiting && Input.GetKey(KeyCode.Return)) t_timer = 0;
		#else
		if(!t_exiting && UnityEngine.N3DS.GamePad.GetButtonHold(N3dsButton.A)){ 
			if(scr_gameInit.globalValues != null){
			if(scr_gameInit.globalValues.isFocused) t_timer = 0;
			} else t_timer = 0;
		}
		#endif
	}
}