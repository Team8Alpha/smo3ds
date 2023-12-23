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
			scr_main.s.nextSpawn = nextSpawn;
			scr_main.s.SetFocus(false);
			scr_loadScene.s.StartScene (nextArea, transitionType);
			scr_main.s.transform.GetChild (1).GetChild (1).gameObject.SetActive (true); //cuz it was broken, wanted to sleep.
			//PlayerWalkDoor ();
		}
	}
	/*public void PlayerWalkDoor() {
		MarioController.s.transform.Translate (0, 0, 4*Time.deltaTime);
	}*/
}