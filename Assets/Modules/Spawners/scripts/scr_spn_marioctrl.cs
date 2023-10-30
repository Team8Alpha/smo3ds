using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_spn_marioctrl : MonoBehaviour {

	// Use this for initialization
	void Update () {
		scr_summon.f_summon.s_player(transform.position, new Vector3(transform.rotation.x, transform.rotation.y, transform.rotation.z));
		Destroy(gameObject);
	}
}
