using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_areaChanger : MonoBehaviour {

	public string nextArea;
	public int nextSpawn=0;
	public int transitionType=2;

	void Start(){
		this.enabled = false;
	}
	void OnTouch(int num){
		if (num == 2) {
			scr_main._f.nextSpawn = nextSpawn;
			scr_main._f.focusOff ();
			scr_loadScene._f.StartScene (nextArea, transitionType);
			scr_main._f.transform.GetChild (1).GetChild (1).gameObject.SetActive (true); //cuz it was broken, wanted to sleep.
			//PlayerWalkDoor ();
		}
	}
	/*public void PlayerWalkDoor() {
		MarioController.marioObject.transform.Translate (0, 0, 4*Time.deltaTime);
	}*/
}