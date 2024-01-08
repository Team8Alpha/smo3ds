using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum eSnd
{
	CappyHacked,
	CappySpin,
	CoinCollect,
	JnMoon64Get,
	JnMoonGet,
	JnMoonGet8Bit,
	JnMoonGrandGet,
	JnMoonMainGet,
	JnNewUnlock,
	JnPreBroodals,
	JnStoryLogo,
	JnSuccess,
	JnSuccessComplete,
	JnSuccessS,
	JnSuccessSS,
	JnWorldIntro,
	JnWorldIntro2,
	JnZeldaItem,
	MarioTitleScream,
	MoonNearby

}

public class scr_manAudio : MonoBehaviour {

	AudioSource mAudioSND;
	AudioSource mAudioBGM;
	public AudioClip[] listSound;
	public static scr_manAudio s;

	void Awake () {
		s = this;
		this.enabled = false;
		mAudioSND = gameObject.GetComponents<AudioSource>()[0];
		mAudioBGM = gameObject.GetComponents<AudioSource>()[1];

		listSound = Resources.LoadAll<AudioClip>("Audio/Sounds");

		LoadSND(new eSnd[] {eSnd.JnSuccess}); // GLOBAL AUDIO LOADING LIST.

	}

	public void PlaySND(eSnd eIdSnd, int _volume = 1)
	{
		AudioClip sndClip = GetSND(eIdSnd);
		mAudioSND.volume = _volume;
		mAudioSND.PlayOneShot(sndClip);
	}
	public void PlaySelfSND(ref AudioSource _mAudio, eSnd eIdSnd, bool isLoop = false, bool isOneShot = false, float _volume = 1)
    {
		_mAudio.enabled = true;
		_mAudio.volume = _volume;
		_mAudio.loop = isLoop;
		AudioClip sndClip = GetSND(eIdSnd);
		if (isOneShot) _mAudio.PlayOneShot(sndClip);
		else
		{
			_mAudio.clip = sndClip;
			_mAudio.Play();
		}
	}

	public AudioClip GetSND(eSnd eIdSnd)
    {
		return listSound[(int)eIdSnd];
	}

	public bool isPlaying(bool isBGM)
    {
		return isBGM ? mAudioBGM.isPlaying : mAudioSND.isPlaying;
    }

	public void PlayBGM(string name, int _volume = 1, bool isLoop = true)
    {
		mAudioBGM.volume = _volume;
		mAudioBGM.loop = isLoop;
		mAudioBGM.clip = Resources.Load<AudioClip>("Audio/Music/bgm" + name);
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
			listSound[(int)sounds[i]].LoadAudioData();
		}
	}
	public void UnloadSND(eSnd[] sounds)
	{
		for (int i = 0; i != sounds.Length; i++)
		{
			listSound[(int)sounds[i]].UnloadAudioData();
		}
	}
	public void LoadSND(eSnd sound) { listSound[(int)sound].LoadAudioData(); }
	public void UnloadSND(eSnd sound) { listSound[(int)sound].UnloadAudioData(); }
}