﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum eSnd
{
	CappyHacked,
	CappySpin,
	CoinCollect,
	JnMoonGet,
	JnSuccess,
	MarioTitleScream

}

public class scr_manAudio : MonoBehaviour {

	AudioSource mAudioSND;
	AudioSource mAudioBGM;
	public scr_listSnd mTableSound;
	public static scr_manAudio _f;

	void Awake () {
		_f = this;
		this.enabled = false;
		mAudioSND = gameObject.GetComponents<AudioSource>()[0];
		mAudioBGM = gameObject.GetComponents<AudioSource>()[1];
		mTableSound = GetComponent<scr_listSnd>();

        LoadSND(new eSnd[] {eSnd.JnSuccess}); // GLOBAL AUDIO LOADING LIST.
		//mTableSound.Init();
	}

	public void PlaySND(eSnd eIdSnd, int _volume = 1, AudioSource _mAudio = null)
    {
		if (_mAudio == null) _mAudio = mAudioSND;
		_mAudio.volume = _volume;

		AudioClip sndClip = mTableSound.tableSound[(int)eIdSnd];
		_mAudio.PlayOneShot(sndClip);

		scr_main.DPrint(sndClip.name + " " + sndClip.loadState);
	}

	public bool isPlaying(bool isBGM)
    {
		return isBGM ? mAudioBGM.isPlaying : mAudioSND.isPlaying;
    }

	public void PlayBGM(string name, int _volume = 1, bool isLoop = true)
    {
		mAudioBGM.volume = _volume;
		mAudioBGM.loop = isLoop;
		mAudioBGM.clip = Resources.Load<AudioClip>("Music/bgm" + name);
		mAudioBGM.Play();

	}
	public void FadeBGM(float time, float targetVolume, bool isStop = true)
    {
		StartCoroutine(StartFade(time, targetVolume, isStop));
	}
	IEnumerator StartFade(float time, float volume, bool isStop)
    {
		float currentTime = 0;
		float startVolume = mAudioBGM.volume;
		float targetVolume = volume;
		while (currentTime < time)
		{
			currentTime += Time.deltaTime;
			mAudioBGM.volume = Mathf.Lerp(startVolume, targetVolume, currentTime / time);
			yield return null;
		}
		if (isStop && mAudioBGM.volume <= 0.1f) mAudioBGM.Stop();
		yield break;
	}

	public void LoadSND(eSnd[] sounds)
	{
		for (int i = 0; i != sounds.Length; i++)
		{
			mTableSound.tableSound[(int)sounds[i]].LoadAudioData();
		}
	}
	public void UnloadSND(eSnd[] sounds)
	{
		for (int i = 0; i != sounds.Length; i++)
		{
			mTableSound.tableSound[(int)sounds[i]].UnloadAudioData();
		}
	}
	public void LoadSND(eSnd sound) { mTableSound.tableSound[(int)sound].LoadAudioData(); }
	public void UnloadSND(eSnd sound) { mTableSound.tableSound[(int)sound].UnloadAudioData(); }
}