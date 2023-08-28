using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class anim_mapRotate : MonoBehaviour {
	
	public float rotationSpeed = 0.0001f;//yep
	
	// Update is called once per frame
	void Update () {
		transform.Rotate(0f, 0f, rotationSpeed);//and thats it, really.
	}
}
