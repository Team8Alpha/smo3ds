using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_capWorldHanger0 : MonoBehaviour {

	public bool isEnabled = false;
	private Material material;
	public bool isWhite = false;
	
	void myLight(){
		Vector4 glassCol;
		if (isWhite) {
			glassCol = new Vector4 (1, 1, 1, 1);
		//	transform.GetChild (0).gameObject.GetComponent<Projector> ().material.color = Color.white;
		} else {
			glassCol = new Vector4 (1, 0.86f, 0.36f, 1);
		//	transform.GetChild (0).gameObject.GetComponent<Projector> ().material.color = Color.yellow;
		}
		material = transform.GetChild(1).gameObject.GetComponent<Renderer>().material;
		transform.GetChild(0).gameObject.SetActive(true);
		material.SetVector("_Color", glassCol);
		material.SetVector("_SpecColor", glassCol);
	}
	
	void Start (){
		if(isEnabled) myLight();
	}
	
	void OnTriggerEnter(Collider collis){
		//CAP! if(collis.gameObject.name == "objMarioCap"&&MarioController.marioObject.cappy.isThrown){
		//CAP! if (!isEnabled){ myLight(); isEnabled = true; }
		//CAP! MarioController.marioObject.cappy.SetState(5);
		//CAP! MarioController.marioObject.cappy.transform.position = new Vector3(transform.position.x, transform.position.y+70*transform.localScale.x, transform.position.z);
		//CAP! }
	}
}
