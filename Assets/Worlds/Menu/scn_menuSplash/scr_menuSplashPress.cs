using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class scr_menuSplashPress : MonoBehaviour {

	[SerializeField] float timer = 5;
	//private bool exiting = false;

	void Start(){
		//scr_fadefull._f.Run (true, 0, 0.04f);
        StartCoroutine(Delay());
    }
	
	void Confirm(){
		Debug.Log ("Loading...");
		//scr_loadScene._f.loadTransition ("scn_capMain0", 1);
		UnityEngine.SceneManagement.SceneManager.LoadScene("scn_menuTitle");
		scr_manageData._f.Load ();
	}

    /*void Update () {
		timer -= 0.1f;
		if(timer<=0){
			if (exiting) {
				
			} else {
				scr_fadefull._f.Run (false, 0, 0.08f);
				timer = 2;
				exiting = true;
			}
		}
		#if UNITY_EDITOR
		if(!exiting && Input.GetKey(KeyCode.Return)) timer = 0;
		#else
		if(!exiting && UnityEngine.N3DS.GamePad.GetButtonHold(N3dsButton.A)){ 
			if(scr_gameInit.globalValues != null){
			if(scr_gameInit.globalValues.isFocused) timer = 0;
			} else timer = 0;
		}
		#endif
	}*/

    IEnumerator Delay()
	{
		yield return new WaitForSecondsRealtime(timer);
		Confirm();
	}
}