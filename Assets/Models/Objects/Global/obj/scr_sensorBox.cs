using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_sensorBox : MonoBehaviour {

	public Vector3 size;
	BoxCollider sensor;
	public string funcEnter = "OnSensorEnter";
	public string funcStay = "OnSensorStay";
	public string funcExit = "OnSensorExit";


	void Start () {
		this.enabled = false;
		sensor = gameObject.AddComponent<BoxCollider> ();
		sensor.size = size;
		sensor.isTrigger = true;
	}

	void OnTriggerEnter(Collider col){
		if(funcEnter != "") transform.parent.gameObject.SendMessage (funcEnter, col);
	}

	void OnTriggerStay(Collider col){
		if(funcStay != "") transform.parent.gameObject.SendMessage (funcStay, col);
	}

	void OnTriggerExit(Collider col){
		if(funcExit != "") transform.parent.gameObject.SendMessage (funcExit, col);
	}
}
