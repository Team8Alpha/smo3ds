using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scrSP_frailBox : MonoBehaviour {

	// Use this for initialization
	void Start () {
		scr_summon.s.s_object (11, transform.position, transform.eulerAngles);
		Destroy(gameObject);
	}
}
