using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_title : MonoBehaviour {
	public Animator anim;
	private AudioSource snd_mTitle;
	private Vector3 marioPos;
	
	// Use this for initialization
	void Start(){
		anim = GetComponent<Animator> ();
		snd_mTitle = GetComponent<AudioSource> ();
		marioPos = transform.position;
		transform.position = new Vector3 (-1000, transform.position.y, transform.position.z);
		scr_fadefull._f.Run (true, 0, 0.05f);
	}
	
	// Update is called once per frame
	void Update () {
		if (scr_fadefull._f.isDone) {
			if (transform.position.x == -1000)
				snd_mTitle.Play ();
			if (transform.position.x < -1004.5) {
				transform.position = marioPos;
				anim.Play ("titleStart");
				this.enabled = false;
			}
			transform.Translate (new Vector3 (0.1f, 0, 0));
		}
	}
}
