using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_spn_moon : MonoBehaviour {

	public string moonName = "Hidden from the Developers";
	public int color = 0;
	// Use this for initialization
	void Update () {
		GameObject obj = scr_summon.f_summon.s_object(6, transform.position, transform.eulerAngles).gameObject;
		scr_behaviorMoon scr = obj.GetComponent<scr_behaviorMoon>();
		scr.moonName=moonName;
		scr.color = color;
		Destroy(gameObject);
	}
}
