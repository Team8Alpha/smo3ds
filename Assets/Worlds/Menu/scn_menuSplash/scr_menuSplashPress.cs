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
		scr_main._f.SetCMD ("Loading...");
		//scr_loadScene._f.StartScene ("scn_capMain0", 1);
		UnityEngine.SceneManagement.SceneManager.LoadScene("scn_menuTitle");
		scr_manageData._f.Load ();
		scr_main._f.SetCMD ("");
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
			if(scr_main._f != null){
			if(scr_main._f.isFocused) timer = 0;
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