using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_manageAudio : MonoBehaviour {

	public static scr_manageAudio _f;
	void Start () {
		_f = this;
	}

	public AudioClip ReturnAudioClip (string input) {
		return Resources.Load<AudioClip>("Audio/Sounds/"+input);
	}
}
