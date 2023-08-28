using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class scr_behaviorMoon : MonoBehaviour {

	//COLOR ?
	public int currentState = 0;
	private Animator anim;
	public string moonName = "ERROR";
	private float rotateAddition = 0;
	private bool bvar0 = false;
	private Vector3 tmpMpos;
	void Start () {
		tmpMpos.x -= 0;
		anim = GetComponent<Animator>();
	}
	void OnTouch(int numType){
		switch (numType) {
		case 1://cap
			rotateAddition = 30;
			break;
		case 2://mar
			currentState = 1;
			MarioController.marioObject.groundedPosition = MarioController.marioObject.transform.position.y;
			tmpMpos = MarioController.marioObject.transform.position;
			anim.Play("getStart");
			GetComponent<Collider>().enabled = false; //or else it literally disables marios collision
			break;
		}
	}

	void Update () {
		switch (currentState) {
		case 0://normal rotate
			if (rotateAddition > 0) {
				transform.Rotate (0, -rotateAddition, 0);
				rotateAddition -= 0.5f;
			}
			transform.Rotate (0, -5.3f, 0);
			break;
		case 1://collected
			if (!bvar0) {
				scr_gameInit.globalValues.focusOff ();
				anim.Play ("get");

				MarioController.marioObject.transform.localRotation = Quaternion.Euler(MarioController.marioObject.transform.rotation.eulerAngles.x, MarioCam.marioCamera.target.transform.rotation.eulerAngles.y+90, MarioController.marioObject.transform.rotation.eulerAngles.z);
				//MarioController.marioObject.transform.position = new Vector3(MarioController.marioObject.transform.position.x, transform.position.y, MarioController.marioObject.transform.position.z);
				//MarioController.marioObject.GetComponent<Rigidbody>().useGravity = false;
				MarioController.marioObject.gameObject.GetComponent<Animator> ().Play ("demoShineGet");
				if (MarioController.marioObject.hasCaptured)
					for (int i = 0; i <= 9; i++) {
						MarioController.marioObject.transform.GetChild (i).gameObject.SetActive (true);
					}
				transform.position = MarioController.marioObject.transform.position;
				transform.Translate (0, 4, 0);
				transform.LookAt (MarioCam.marioCamera.transform);
				transform.Translate (0, -4, 0);

				string t_date = System.DateTime.UtcNow.ToShortDateString();
				scr_gameInit.globalValues.moonsCount++;
				scr_gameInit.globalValues.transform.GetChild (2).transform.GetChild (0).gameObject.SetActive (true);
				scr_gameInit.globalValues.transform.GetChild (2).transform.GetChild (0).GetChild (1).gameObject.GetComponent<Text> ().text = moonName;
				scr_gameInit.globalValues.transform.GetChild (2).transform.GetChild (0).GetChild (2).gameObject.GetComponent<Text> ().text = t_date;
				bvar0 = true;
					
				currentState = 2;
			}
			break;
		case 2://finishing
			if (anim.GetCurrentAnimatorStateInfo (0).normalizedTime > 1) {
				Destroy (gameObject);
				scr_gameInit.globalValues.focusOn ();
				MarioCam.marioCamera.setState (0, MarioCam.marioCamera.transform.position, MarioCam.marioCamera.transform.rotation);
				MarioCam.marioCamera.isLocked = false;
				MarioController.marioObject.gameObject.GetComponent<Animator> ().Play ("default");
				scr_gameInit.globalValues.transform.GetChild (2).transform.GetChild (0).gameObject.SetActive (false);
				MarioController.marioObject.GetComponent<Rigidbody> ().useGravity = true;
				if (MarioController.marioObject.hasCaptured)
					for (int i = 0; i <= 9; i++) {
						MarioController.marioObject.transform.GetChild (i).gameObject.SetActive (false);
					}
			}
			break;
		}
	}
}