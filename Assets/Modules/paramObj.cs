using UnityEngine;
using System;

public class paramObj : MonoBehaviour {

	[SerializeField]
	public float posCenterV = 0;
	public bool isTouch = false;
	public bool isCapTrigger = false;
	public bool isHack = true;
	public bool isLOD = true;

	public float GetPosCenterV()
    {
		return posCenterV + transform.position.y;
    }
	void Awake(){
		if (isLOD) MarioController.s.SetLOD(gameObject, false);
		this.enabled = false;
	}
}
