using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_sensorBox : MonoBehaviour {

	BoxCollider sensor;
	public string funcEnter = "OnSensorEnter";
	public string funcStay = "OnSensorStay";
	public string funcExit = "OnSensorExit";


	void Start () {
		this.enabled = false;
		sensor = gameObject.GetComponent<BoxCollider> ();
		sensor.isTrigger = true;
	}

	void OnTriggerEnter(Collider coll){
		DoCall (coll, funcEnter);
	}
	void OnTriggerStay(Collider coll){
		DoCall (coll, funcStay);
	}
	void OnTriggerExit(Collider coll){
		DoCall (coll, funcExit);
	}

	void DoCall(Collider coll, string func){
		if(func != "" && coll.gameObject != transform.parent.gameObject) transform.parent.gameObject.SendMessage (func, coll);
	}
}
