using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_sensor : MonoBehaviour {

	Collider sensor;
	public string funcEnter = "OnSensorEnter";
	public string funcStay = "OnSensorStay";
	public string funcExit = "OnSensorExit";
	public GameObject targetObj;
	bool started = false;


	void Start () {
		this.enabled = false;
		if ((sensor = gameObject.GetComponent<Collider>()) == null)
		{
			this.enabled = false;
			return;
		}
		sensor.isTrigger = true;
	}

	void OnTriggerEnter(Collider coll){
		DoCall (coll, funcEnter);
		if (!started)
			started = true;
	}
	void OnTriggerStay(Collider coll){
		if (!started) {
			DoCall (coll, funcEnter);
			started = true;
		}
		DoCall (coll, funcStay);
	}
	void OnTriggerExit(Collider coll){
		DoCall (coll, funcExit);
	}

	void DoCall(Collider coll, string func){
		if(func != "" && coll.gameObject != targetObj) targetObj.SendMessage (func, coll);
	}
}
