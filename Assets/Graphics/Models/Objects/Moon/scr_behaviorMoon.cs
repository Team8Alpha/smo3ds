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
	public int color = 0;
	SkinnedMeshRenderer mat_color;
	Transform globalCanvas;

	void setColor(){
		Color t_color = Color.white;
		Color t_fresnelCol = Color.white;
		switch (color) {
		case 0:
			t_color = new Color (0.91f, 0.9f, 0.2f, 1);
			t_fresnelCol = new Color (0.91f, 0.9f, 0.2f, 1);
			break;
		case 1:
			t_color = new Color (0.6f, 0.33f, 0.26f, 1);
			break;
		case 2:
			t_color = new Color (0.11f, 0.25f, 0.9f, 1);
			break;
		case 3:
			t_color = new Color (0.255f, 0.8f, 0.85f, 1);
			break;
		case 4:
			t_color = new Color (0.24f, 0.87f, 0.4f, 1);
			t_fresnelCol = new Color (0.91f, 0.9f, 0.2f, 1);
			break;
		case 5:
			t_color = new Color (0.9f, 0.45f, 0.18f, 1);
			t_fresnelCol = new Color (0.91f, 0.9f, 0.2f, 1);
			break;
		case 6:
			t_color = new Color (0.82f, 0.165f, 0.192f, 1);
			break;
		case 7:
			t_color = new Color (0.94f, 0.584f, 0.58f, 1);
			t_fresnelCol = new Color (0.94f, 0.584f, 0.58f, 1);
			break;
		case 8:
			t_color = new Color (0.74f, 0.48f, 0.945f, 1);
			t_fresnelCol = new Color (0.74f, 0.48f, 0.945f, 1);
			break;
		case 9:
			t_color = new Color (0.9f, 0.85f, 0.666f, 1);
			t_fresnelCol = new Color (0.9f, 0.85f, 0.666f, 1);
			break;
		}
		mat_color.material.SetColor("_Color", t_color);
		mat_color.material.SetColor("_SpecColor", t_fresnelCol);
	}
	void Start () {
		tmpMpos.x -= 0;
		anim = GetComponent<Animator>();
		mat_color = transform.GetChild(1).GetChild(1).GetComponent<SkinnedMeshRenderer>();
		setColor ();
		globalCanvas = scr_main._f.transform.GetChild (1).transform.GetChild (1);
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
				scr_main._f.focusOff ();
				anim.Play ("get");
				Transform player = MarioController.marioObject.transform;

				MarioCam.marioCamera.confYOffset = 3+player.position.y - MarioController.marioObject.groundedPosition;
				MarioCam.marioCamera.confRotate = false;
				MarioCam.marioCamera.confStickXmax = 0;
				MarioCam.marioCamera.confStickYmax = 0;
				MarioCam.marioCamera.confSmoothTime = 0.3f;
				player.rotation = Quaternion.Euler(player.eulerAngles.x, MarioCam.marioCamera.transform.eulerAngles.y+180, player.eulerAngles.z);
				player.position = new Vector3(player.position.x, transform.position.y, player.position.z);
				transform.position = player.position;
				transform.rotation = player.rotation;

				MarioController.marioObject.gameObject.GetComponent<Animator> ().Play ("demoShineGet");
				if (MarioController.marioObject.hasCaptured)
					for (int i = 0; i <= 8; i++) {
						if (i != 2 && i != 5 && i != 7)
							MarioController.marioObject.transform.GetChild (i).gameObject.SetActive (true);
					}

				scr_manageAudio._f.AudioStart ("Music/Jingle/snd_JingleMoonCollect", false);

				string t_date = System.DateTime.UtcNow.ToShortDateString(); //even works on 3ds
				scr_main._f.moonsCount++;
				globalCanvas.gameObject.SetActive (true);
				globalCanvas.GetChild (1).gameObject.GetComponent<Text> ().text = moonName;
				globalCanvas.GetChild (2).gameObject.GetComponent<Text> ().text = t_date;
				bvar0 = true;
					
				currentState = 2;
			}
			break;
		case 2://finishing
			if (!scr_manageAudio._f.isPlaying()) {
				Destroy (gameObject);
				scr_main._f.focusOn ();
				MarioController.marioObject.rb.velocity.Set(0, 0, 0);
				MarioController.marioObject.SetAnim ("wait", 0.1f);
				MarioController.marioObject.SetState (plState.Falling);
				globalCanvas.gameObject.SetActive (false);
				MarioCam.marioCamera.confYOffset = 2;
				MarioCam.marioCamera.confRotate = true;
				MarioCam.marioCamera.confStickXmax = 1;
				MarioCam.marioCamera.confStickYmax = 1;
				MarioCam.marioCamera.confSmoothTime = 0.5f;
				MarioController.marioObject.GetComponent<Rigidbody> ().useGravity = true;
				if (MarioController.marioObject.hasCaptured)
					for (int i = 0; i <= 8; i++) {
						if (i != 2 && i != 5 && i != 7)
							MarioController.marioObject.transform.GetChild (i).gameObject.SetActive (false);
					}
			}
			break;
		}
	}
}