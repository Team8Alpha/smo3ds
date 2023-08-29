using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_behaviorCoin : MonoBehaviour {

	public int currentState = 0;
	AudioSource sndSrc;
	Animator anim;

	void Start () {
		sndSrc = GetComponent<AudioSource> ();
		anim = GetComponent<Animator> ();
		if (currentState == 1) anim.Play ("collect");
		this.enabled = false;
	}
	void doKill(){
		Destroy (gameObject);
	}
	void OnTouch(int numType){
		if(currentState==0) if(numType == 1 || numType == 2){
			this.enabled = true;
			scr_gameInit.globalValues.coinsCount++;//add coin to global
			sndSrc.Play();
			scr_manageEffect._f.Play("prt_coinSpark0", transform.position, transform.rotation, "prt_coinSpark1");
			StartCoroutine (WaitKill ());
			transform.GetComponent<MeshRenderer> ().enabled = false;
		}
	}
	public IEnumerator WaitKill() {
		yield return new WaitWhile(() => sndSrc.isPlaying);
		Destroy(gameObject);
	}
}
