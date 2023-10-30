using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_spn_coin_single : MonoBehaviour {

	public int currentState = 0;
	// Use this for initialization
	void Start () {
		scr_summon.f_summon.s_object(0, transform.position, transform.eulerAngles).GetComponent<scr_behaviorCoin>().currentState = currentState;
		Destroy(gameObject);
	}
}
