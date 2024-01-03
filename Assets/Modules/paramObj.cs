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
	void Start(){
		if (isLOD) scr_main.s.SetLOD(gameObject, false);
		this.enabled = false;
	}
}
