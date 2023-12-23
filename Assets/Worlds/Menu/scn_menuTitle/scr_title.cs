using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class scr_title : MonoBehaviour {

	private Animator anim;
	private Vector3 marioPos;

	public GameObject spr_rotMap;
	public GameObject spr_shade;
	public Transform cnv_down;
	Material mat_rotMap;
	Material mat_shade;
	int timerShow = 0;
	const int cJumpWaitTime = 45 + 15;

	public float speed;
	public Color startColor, endColor;
	public Color startColorShade, endColorShade;
	public GameObject buttonRes;//resume button

	public static scr_title s;


	public IEnumerator ChangeEngineColour()
	{
		float tick = 0f;
		while (mat_rotMap.color != endColor)
		{
			tick += Time.unscaledDeltaTime * speed;
			mat_rotMap.color = Color.Lerp(startColor, endColor, tick);
			mat_shade.color = Color.Lerp(startColorShade, endColorShade, tick);
			yield return null;
		}
	}

	// Use this for initialization
	void Start()
	{
		anim = GetComponent<Animator>();

		marioPos = transform.position; // to move him back later
		transform.position = new Vector3(-1000, transform.position.y, transform.position.z); //move to waitzone, works like a timer.

		scr_fadefull.s.Run(true, 0, 0.02f);//fade in
		mat_rotMap = spr_rotMap.gameObject.GetComponent<MeshRenderer>().material;
		mat_shade = spr_shade.gameObject.GetComponent<MeshRenderer>().material;//get materials

		scr_main.s.SetFocus(false);
		s = this;

		scr_manAudio.s.LoadSND(eSnd.MarioTitleScream);
	}

	// Update is called once per frame
	void Update()
	{
		if(timerShow <= cJumpWaitTime || scr_manAudio.s.isPlaying(false))
		{ // timer (1.5 seconds, since targetFPS is X frames per second...)
			switch (timerShow) {
				case cJumpWaitTime:
					transform.position = marioPos;
					anim.Play("titleStart");
					break;
				case 15:
					scr_manAudio.s.PlaySND(eSnd.MarioTitleScream);
					break;
			}
			timerShow++;
		}
		else
		{ // audio has finished, show canvas
			Finish();
		}
	}
	void Finish()
	{
		scr_main.s.SetFocus(true);
		for (int i = 0; i < 4; i++)
			cnv_down.GetChild(i).gameObject.SetActive(true);
		EventSystem.current.SetSelectedGameObject(buttonRes);
		scr_manAudio.s.PlayBGM("Title");
		scr_manAudio.s.UnloadSND(eSnd.MarioTitleScream);
		this.enabled = false;
	}
}

