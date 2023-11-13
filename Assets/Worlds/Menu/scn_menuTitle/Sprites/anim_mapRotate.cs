using System.Collections;
using UnityEngine;

public class anim_mapRotate : MonoBehaviour {
	
	public float rotationSpeed = 0.02f;

	void Update () {
		transform.Rotate(0f, rotationSpeed, 0f);
	}
}