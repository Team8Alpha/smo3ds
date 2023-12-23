using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class scr_menuSplashPress : MonoBehaviour {

	[SerializeField] float timer = 5;

	void Start(){
		//scr_fadefull.s.Run (true, 0, 0.04f);
        StartCoroutine(Delay());
    }
	
	void Confirm(){
		scr_main.DPrint ("Loading...");
		//scr_loadScene.s.StartScene ("scn_capMain0", 1);
		UnityEngine.SceneManagement.SceneManager.LoadScene("scn_menuTitle");
		scr_manageData.s.Load ();
		scr_main.DPrint ("");
	}

	void Update()
    {
#if isRelease
			this.enabled = false;
#else
			transform.parent.GetChild(2).GetChild(1).gameObject.SetActive(true);
#endif
    }

    /*void Update () {
		timer -= 0.1f;
		if(timer<=0){
			if (exiting) {
				
			} else {
				scr_fadefull.s.Run (false, 0, 0.08f);
				timer = 2;
				exiting = true;
			}
		}
#if UNITY_EDITOR
		if(!exiting && Input.GetKey(KeyCode.Return)) timer = 0;
#else
		if(!exiting && UnityEngine.N3DS.GamePad.GetButtonHold(N3dsButton.A)){ 
			if(scr_main.s != null){
			if(scr_main.s.isFocused) timer = 0;
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