using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_manageEffect : MonoBehaviour {

	private ParticleSystem currentPrt;
	private string nextPrt0;
	private string nextPrt1;
	private bool isRunning = false;
	public static scr_manageEffect _f;
	void Start(){
		_f = this;
	}
	// 23.85809  1.9123  133.5931  -90  0.0165
	public void Play(string namePrt, Vector3 prtPos, Quaternion prtRot, string _nextPrt0="null", string _nextPrt1="null"){
		currentPrt = Instantiate (Resources.Load<ParticleSystem>("Effects/" + namePrt), prtPos, prtRot);
		nextPrt0 = _nextPrt0;
		nextPrt1 = _nextPrt1;
		isRunning = true;
		if (currentPrt.gameObject != null) {
			currentPrt.gameObject.GetComponent<ParticleSystem> ().Play ();
		} else
			scr_main._f.SetCMD ("E: PRT." + namePrt + "NOT FOUND");
	}
	void Update(){
		if (isRunning) {
			if (!currentPrt.isPlaying) {
				Transform tmp_001 = currentPrt.transform;
				Destroy (currentPrt.gameObject);
				currentPrt = null;
				if (nextPrt0 != "null") {
					currentPrt = null;
					Play (nextPrt0, tmp_001.position, tmp_001.rotation);
					nextPrt0 = nextPrt1;
				} else {
					isRunning = false;
					currentPrt = null;
				}
			}
			currentPrt.transform.rotation = MarioCam.marioCamera.transform.rotation;
		}
	}
}