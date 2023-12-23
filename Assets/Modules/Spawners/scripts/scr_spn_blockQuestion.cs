using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_spn_blockQuestion : MonoBehaviour {

	public int avgCoins = 1;
	// Use this for initialization
	void Awake () {
		scr_summon.s.s_object(7, transform.position, transform.eulerAngles).GetComponent<scr_behaviorBlockQuestion>().FrameLimit = avgCoins;
		Destroy(gameObject);
	}
}
