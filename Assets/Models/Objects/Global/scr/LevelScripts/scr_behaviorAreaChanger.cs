using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_behaviorAreaChanger : MonoBehaviour {

	public string nextArea;
	public int nextSpawn=0;
	public int transitionType=2;

	void OnTouch(int num){
		if (num == 2) {
			scr_gameInit.globalValues.nextSpawn = nextSpawn;
			scr_gameInit.globalValues.focusOff ();
			scr_loadScene._f.loadTransition (nextArea, transitionType);
			PlayerWalkDoor ();
		}
	}
	public void PlayerWalkDoor() {
		MarioController.marioObject.transform.Translate (0, 0, 4*Time.deltaTime);
	}
}