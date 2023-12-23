using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_spn_blockVoid : MonoBehaviour {

	void Awake () {
		scr_summon.s.s_object(9, transform.position, transform.eulerAngles);
		Destroy(gameObject);
	}
}
