using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_sensor : MonoBehaviour {

	public float radius;
	public Vector3 offset;
	SphereCollider sensor;
	public string funcEnter = "OnSensorEnter";
	public string funcExit = "OnSensorExit";


	void Start () {
		this.enabled = false;
		sensor = gameObject.AddComponent<SphereCollider> ();
		sensor.radius = radius;
		sensor.center = offset;
		sensor.isTrigger = true;
	}

	void OnTriggerEnter(Collider col){
		transform.parent.gameObject.SendMessage (funcEnter, col);
	}

	void OnTriggerExit(Collider col){
		transform.parent.gameObject.SendMessage (funcExit, col);
	}
}
