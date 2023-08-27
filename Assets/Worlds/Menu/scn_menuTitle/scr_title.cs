using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_title : MonoBehaviour {
	public Animator anim;
	private AudioSource snd_mTitle;
	private Vector3 marioPos;
	
	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator>();
		snd_mTitle = GetComponent<AudioSource>();
		marioPos = transform.position;
		transform.position = new Vector3(-500,transform.position.y,transform.position.z);
		snd_mTitle.Play();
	}
	
	// Update is called once per frame
	void Update () {
		if(transform.position.x < -503.6f && transform.position.x > -503.7f){
			transform.position = marioPos;
			anim.Play("titleStart");
		}
		if(transform.position.x < 0) transform.Translate(new Vector3(0.1f,0,0));
	}
}
