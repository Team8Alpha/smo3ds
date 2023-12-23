using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_spn_goomba : MonoBehaviour {

	public int stackAmount = 1;
	public bool isMoving = true;
	public int currentState = 0;
	// Use this for initialization
	void Update () {
		GameObject goombaStacked = scr_summon.s.s_entity(0, transform.position, new Vector3(transform.rotation.x, transform.rotation.y, transform.rotation.z));
		scr_behaviorGoomba goombaSta = goombaStacked.GetComponent<scr_behaviorGoomba>();
		goombaSta.isMoving = isMoving;
		goombaSta.stackAmount = stackAmount;
		goombaSta.currentState = currentState;
		Destroy(gameObject);
	}
}
