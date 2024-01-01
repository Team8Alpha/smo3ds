using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_behaviorCoin : MonoBehaviour {

	public int currentState = 0;
	Animator anim;

	GameObject childMesh; // Reference to the child mesh GameObject

	void Start ()
	{
		childMesh = transform.GetChild(1).gameObject;
		anim = childMesh.GetComponent<Animator> ();
		if (currentState == 1)
		{
			//anim.Play("collect");
			scr_main.s.coinsCount++;//add coin to global
			scr_manAudio.s.PlaySND(eSnd.CoinCollect);
			transform.GetComponent<Collider>().enabled = false;
			doKill();
		}
		else this.enabled = false;
	}

	void doKill(){
		Destroy (gameObject);
	}

	void OnTouch(int numType) {
		if (currentState == 0) if (numType == 1 || numType == 2) {
				scr_main.s.coinsCount++;//add coin to global
				scr_manAudio.s.PlaySND(eSnd.CoinCollect);
				scr_manageEffect.s.Play("prt_coinSpark0", transform.position, transform.rotation, "prt_coinSpark1");
				doKill();
			}
	}
	void Update()
    {
		if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 1) doKill();
    }
}