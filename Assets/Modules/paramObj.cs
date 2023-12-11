using UnityEngine;
using System;

public class paramObj : MonoBehaviour {

	[SerializeField]
	public float posCenterV = 0;
	public bool isTouch = false;
	public bool isCapture = false;
	public bool isHack = true;
	public bool isLOD = true;

	public float GetPosCenterV()
    {
		return posCenterV + transform.position.y;
    }
	void Awake(){
		if (isLOD && transform.GetChild(1) != null) transform.GetChild(1).gameObject.SetActive(false);
		this.enabled = false;
	}
}