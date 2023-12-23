using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MoonColor {
	yellow=0,
	brown,
	dblue,
	lblue,
	green,
	orange,
	red,
	pink,
	purple,
	moon,
	white
}

public class scr_spn_moon : MonoBehaviour {

	public string moonName = "Hidden from the Developers";
	public MoonColor color = MoonColor.yellow;
	// Use this for initialization
	void Update () {
		GameObject obj = scr_summon.s.s_object(6, transform.position, transform.eulerAngles).gameObject;
		scr_behaviorMoon scr = obj.GetComponent<scr_behaviorMoon>();
		scr.moonName=moonName;
		scr.color = (int) color;
		Destroy(gameObject);
	}
}
