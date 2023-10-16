using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TESTING : MonoBehaviour {

	public Rigidbody rb;
	public ForceMode fm;
	public float value = 0;
	public float MAXIMAL = 0;

	void Start(){
		rb = gameObject.GetComponent<Rigidbody> ();
	}
	void Update () {
		if (transform.position.y > MAXIMAL)
			MAXIMAL = transform.position.y;
		if(Input.GetKeyUp(KeyCode.Space)) rb.AddForce (value * Vector3.up, fm);
		if (Input.GetKeyUp (KeyCode.C))
			MAXIMAL = 0;
		if (Input.GetKeyUp (KeyCode.A))
			Debug.Log (MAXIMAL); 
	}
}
