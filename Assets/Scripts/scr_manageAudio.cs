using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_manageAudio : MonoBehaviour {

	AudioSource sndSrc;
	AudioClip clip;
	public static scr_manageAudio _f;
	float fadeSpeed = 1f;
	bool isFadeOut = false;

	void Start () {
		_f = this;
		this.enabled = false;
		sndSrc = gameObject.GetComponent<AudioSource> ();
	}
	void Update(){
		if (isFadeOut) {
			if (sndSrc.volume > 0)
				sndSrc.volume -= fadeSpeed * Time.deltaTime;
			else {
				sndSrc.volume = 1;
				sndSrc.Stop ();
				this.enabled = false;
			}
		} else {
			if (sndSrc.volume < 1)
				sndSrc.volume += fadeSpeed * Time.deltaTime;
			else
				this.enabled = false;
		}
	}

	public AudioClip GetClip (string input) {
		return Resources.Load<AudioClip>("Audio/"+input);
	}
	public bool PlayClip (string input){
		if ((clip = GetClip (input)) == null) {
			scr_main.DPrint ("<SND> ERR! not a valid sound clip: Audio/"+input);
			return false;
		}
		sndSrc.PlayOneShot (clip);
		return true;
	}
	public bool AudioStart (string input, bool isLoop = true){
		if ((clip = GetClip (input)) == null) {
			scr_main.DPrint ("<SND> ERR! not a valid sound clip: Audio/"+input);
			return false;
		}
		sndSrc.clip = clip;
		sndSrc.loop = isLoop;
		sndSrc.Play ();
		return true;
	}
	public void AudioStop (string input){
		sndSrc.Stop ();
	}
	public bool AudioFadeIn (string Input, float speed, bool isLoop = true){
		if (!AudioStart (Input, isLoop))
			return false;
		sndSrc.volume = 0;
		AudioFade (speed, false);
		return true;
	}
	public void AudioFadeOut (float speed){
		AudioFade (speed, true);
	}
	void AudioFade(float speed, bool isFadeOut){
		this.fadeSpeed = speed;
		this.isFadeOut = isFadeOut;
		this.enabled = true;
	}
	public void SetVolume (float volume){
		sndSrc.volume = volume;
	}

	public bool isPlaying() { return sndSrc.isPlaying; }
}
