using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scrA_die : StateMachineBehaviour {
	override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		if(stateInfo.normalizedTime >= 1f) animator.SetBool("dead" , true);
		if(stateInfo.normalizedTime*29 >= 1f) animator.SetBool("flat" , true);
	}
}
