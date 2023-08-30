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

	void setColor(){
		Color t_color = Color.white;
		switch (color) {
		case 0:
			t_color = new Color (0.9490196078431372f, 0.8470588235294118f, 0.30980392156862746f, 1);
			break;
		case 1:
			t_color = new Color (0.6823529411764706f, 0.8156862745098039f, 0.3686274509803922f, 1);
			break;
		case 2:
			t_color = new Color (0.40784313725490196f, 0.803921568627451f, 0.796078431372549f, 1);
			break;
		}
		mat_color.material.color = t_color;
	}
	void Start () {
		tmpMpos.x -= 0;
		anim = GetComponent<Animator>();
		mat_color = transform.GetChild(1).GetComponent<SkinnedMeshRenderer>();
		setColor ();
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
				Transform player = MarioController.marioObject.transform;
				player.localRotation = Quaternion.Euler(player.rotation.eulerAngles.x, MarioCam.marioCamera.target.eulerAngles.y-180, player.eulerAngles.z);
				player.position = new Vector3(player.position.x, transform.position.y, player.position.z);
				MarioController.marioObject.gameObject.GetComponent<Animator> ().Play ("demoShineGet");
				if (MarioController.marioObject.hasCaptured)
					for (int i = 0; i <= 8; i++) {
						MarioController.marioObject.transform.GetChild (i).gameObject.SetActive (true);
					}
				transform.position = player.position;
				transform.rotation = player.rotation;
				MarioCam.marioCamera.setState (1, new Vector3(player.position.x, player.position.y+2, player.position.z),
					Quaternion.Euler(player.eulerAngles.x, player.eulerAngles.y+180, player.eulerAngles.z));

				string t_date = System.DateTime.UtcNow.ToShortDateString(); //even works on 3ds
				Transform globalCanvas = scr_gameInit.globalValues.transform.GetChild (3).transform.GetChild (0);
				scr_gameInit.globalValues.moonsCount++;
				globalCanvas.gameObject.SetActive (true);
				globalCanvas.GetChild (1).gameObject.GetComponent<Text> ().text = moonName;
				globalCanvas.GetChild (2).gameObject.GetComponent<Text> ().text = t_date;
				bvar0 = true;
					
				currentState = 2;
			}
			break;
		case 2://finishing
			if (anim.GetCurrentAnimatorStateInfo (0).normalizedTime > 1) {
				Destroy (gameObject);
				scr_gameInit.globalValues.focusOn ();
				MarioCam.marioCamera.setState (0, MarioCam.marioCamera.target.position, MarioCam.marioCamera.target.rotation);
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