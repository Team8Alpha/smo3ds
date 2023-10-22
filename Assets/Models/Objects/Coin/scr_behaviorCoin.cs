using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_behaviorCoin : MonoBehaviour {

	public int currentState = 0;
	AudioSource sndSrc;
	Animator anim;

	GameObject childMesh; // Reference to the child mesh GameObject

	void Start () {
		childMesh = transform.GetChild (0).gameObject;
		sndSrc = childMesh.GetComponent<AudioSource> ();
		anim = childMesh.GetComponent<Animator> ();
		if (currentState == 1) anim.Play ("collect");
		this.enabled = false;
	}

	void doKill(){
		scr_main._f.coinsCount++;
		Destroy (gameObject);
	}

	void OnTouch(int numType){
		if(currentState==0) if(numType == 1 || numType == 2){
			transform.GetComponent<Collider> ().enabled = false;
			scr_main._f.coinsCount++;//add coin to global
			sndSrc.Play();
			scr_manageEffect._f.Play("prt_coinSpark0", transform.position, transform.rotation, "prt_coinSpark1");
			StartCoroutine (WaitKill ());
			childMesh.GetComponent<MeshRenderer> ().enabled = false;
		}
	}

	public IEnumerator WaitKill() {
		yield return new WaitWhile (() => sndSrc.isPlaying);
		Destroy (gameObject);
	}
}