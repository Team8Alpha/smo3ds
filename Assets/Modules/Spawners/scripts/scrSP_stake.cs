using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scrSP_stake : MonoBehaviour {

	// Use this for initialization
	void Start () {
		scr_summon.f_summon.s_object (10, transform.position, transform.eulerAngles);
		Destroy(gameObject);
	}
}
