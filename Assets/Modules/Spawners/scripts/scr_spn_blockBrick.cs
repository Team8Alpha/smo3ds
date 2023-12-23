using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_spn_blockBrick : MonoBehaviour {

	public int avgCoins = 0;
	// Use this for initialization
	void Awake () {
		scr_summon.s.s_object(8, transform.position, transform.eulerAngles).GetComponent<scr_behaviorBlockBrick>().FrameLimit = avgCoins;
		Destroy(gameObject);
	}
}
