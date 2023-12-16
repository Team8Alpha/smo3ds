using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum eSnd
{
	CoinCollect = 0,
	MarioTitleScream,
	CappySpin,
	CappyHacked,
	JnMoonGet,
	JnSuccess

}

public class scr_manAudio : MonoBehaviour {

	AudioSource mAudioSND;
	AudioSource mAudioBGM;
	public scr_tableSnd objSoundTable;
	public static scr_manAudio _f;
	public AudioClip[] dataJump;

	void Awake () {
		_f = this;
		this.enabled = false;
		mAudioSND = gameObject.GetComponents<AudioSource>()[0];
		mAudioBGM = gameObject.GetComponents<AudioSource>()[1];
		//objSoundTable = Resources.Load<scr_tableSnd>("objSoundTable");
		objSoundTable = objSoundTable.GetComponent<scr_tableSnd>();
	}

	public void PlaySND(eSnd eIdSnd, int _volume = 1, AudioSource _mAudio = null)
    {
		if (_mAudio == null) _mAudio = mAudioSND;
		_mAudio.volume = _volume;
		AudioClip sndClip = objSoundTable.tableSound[(int)eIdSnd];
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
}
