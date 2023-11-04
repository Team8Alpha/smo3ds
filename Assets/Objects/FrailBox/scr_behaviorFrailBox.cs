using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_behaviorFrailBox : MonoBehaviour {

	Animator anim;
	int currentState;
	int hitCount;
	int hitMax = 14;
	void Start () {
		anim = GetComponent<Animator> ();
		this.enabled = false;
	}
	public void OnTouch(int num){
		if (num == 1) {
			hitCount++;
			if (hitCount == hitMax) toBreak ();
		}
	}

	void Update(){
		transform.Translate (0, -0.2f, 0);
	}
	void OnSensorGroundEnter(Collider coll){
		if (coll.gameObject.layer != 20)
			this.enabled = false;
	}
	void OnSensorGroundExit(Collider coll){
		if (coll.gameObject.layer != 20)
			this.enabled = true;
	}
	void OnSensorInsideStay(Collider coll){
		if (coll.gameObject.layer != 20)
			transform.Translate (0, 0.3f, 0);
	}

	void toBreak(){
		hitCount = 0;
		anim.CrossFade ("damage", 0.1f);
		switch (currentState) {
		case 1://1
			transform.GetChild (1).GetChild (0).gameObject.SetActive (false);
			transform.GetChild (1).GetChild (1).gameObject.SetActive (true);
			transform.GetChild (1).GetChild (2).gameObject.SetActive (true);
			break;
		case 2://2
			transform.GetChild (1).GetChild (1).gameObject.SetActive (false);
			hitCount = hitMax-2;
			break;
		case 3:
			GameObject.Instantiate (Resources.Load<GameObject> ("Objects/objFrailBoxTrace"), transform.position, transform.rotation);
			Destroy (gameObject);
			break;
		}
		currentState++;
	}
}
