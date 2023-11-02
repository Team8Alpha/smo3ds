using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_behaviorCapHanger : MonoBehaviour {

	public int myType = 0;
	public int myAmount = 1;
	public bool isEnabled = false;
	private Material material;
	public bool isWhite = false;
	public float yOff = 0;
	public float zOff = 0;

	void Start (){
		if(isEnabled) myLight();
		this.enabled = false;
	}

	void myLight(){
		Vector4 glassCol;
		if (isWhite) {
			glassCol = new Vector4 (1, 1, 1, 1);
		} else {
			glassCol = new Vector4 (1, 0.86f, 0.36f, 1);
		}
		material = transform.GetChild(1).GetChild(0).gameObject.GetComponent<Renderer>().material;
		material.SetVector("_Color", glassCol);
		material.SetVector("_SpecColor", glassCol);
	}

	void myTrigger(){
		switch (myType) {
		case 0://just spin
			break;
		case 1://coin/s
			break;
		case 2://moon spin
			break;
		case 3://lamplight
			break;
		case 4:
			break;
		}
	}

	public void OnTouch(int numType){
		if (numType == 1) {
			if(!isEnabled){ myLight(); isEnabled = true; }
			myTrigger ();
			MarioController.marioObject.cappy.SetState(5);
			MarioController.marioObject.cappy.transform.position = new Vector3(transform.position.x, transform.position.y+yOff*transform.localScale.x, transform.position.z+zOff);
		}
	}
}
