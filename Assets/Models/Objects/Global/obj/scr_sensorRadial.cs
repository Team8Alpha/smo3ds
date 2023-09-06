using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_sensorRadial : MonoBehaviour {

	public float radius;
	SphereCollider sensor;
	public string funcEnter = "OnSensorEnter";
	public string funcExit = "OnSensorExit";


	void Start () {
		this.enabled = false;
		sensor = gameObject.AddComponent<SphereCollider> ();
		sensor.radius = radius;
		sensor.isTrigger = true;
	}

	void OnTriggerEnter(Collider col){
		if(funcEnter != "") transform.parent.gameObject.SendMessage (funcEnter, col);
	}

	void OnTriggerExit(Collider col){
		if(funcExit != "") transform.parent.gameObject.SendMessage (funcExit, col);
	}
}
