using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class scr_behaviorMoon : MonoBehaviour
{

	//COLOR ?
	public int currentState = 0;
	private Animator anim;
	public string moonName = "ERROR";
	private float rotateAddition = 0;
	public int color = 0;
	SkinnedMeshRenderer mat_color;

	void setColor()
	{
		Color t_color = Color.white;
		Color t_fresnelCol = Color.white;
		switch (color)
		{
			case 0:
				t_color = new Color(0.91f, 0.9f, 0.2f, 1);
				t_fresnelCol = new Color(0.91f, 0.9f, 0.2f, 1);
				break;
			case 1:
				t_color = new Color(0.6f, 0.33f, 0.26f, 1);
				break;
			case 2:
				t_color = new Color(0.11f, 0.25f, 0.9f, 1);
				break;
			case 3:
				t_color = new Color(0.255f, 0.8f, 0.85f, 1);
				break;
			case 4:
				t_color = new Color(0.24f, 0.87f, 0.4f, 1);
				t_fresnelCol = new Color(0.91f, 0.9f, 0.2f, 1);
				break;
			case 5:
				t_color = new Color(0.9f, 0.45f, 0.18f, 1);
				t_fresnelCol = new Color(0.91f, 0.9f, 0.2f, 1);
				break;
			case 6:
				t_color = new Color(0.82f, 0.165f, 0.192f, 1);
				break;
			case 7:
				t_color = new Color(0.94f, 0.584f, 0.58f, 1);
				t_fresnelCol = new Color(0.94f, 0.584f, 0.58f, 1);
				break;
			case 8:
				t_color = new Color(0.74f, 0.48f, 0.945f, 1);
				t_fresnelCol = new Color(0.74f, 0.48f, 0.945f, 1);
				break;
			case 9:
				t_color = new Color(0.9f, 0.85f, 0.666f, 1);
				t_fresnelCol = new Color(0.9f, 0.85f, 0.666f, 1);
				break;
		}
		mat_color.material.SetColor("_Color", t_color);
		mat_color.material.SetColor("_SpecColor", t_fresnelCol);
	}
	void Start()
	{
		anim = GetComponent<Animator>();
		mat_color = transform.GetChild(1).GetChild(0).GetComponent<SkinnedMeshRenderer>();
		setColor();
	}
	void OnTouch(int numType)
	{
		switch (numType)
		{
			case 1://cap
				rotateAddition = 1200;
				break;
			case 2://mar
				currentState = 1;
				GetComponent<Collider>().enabled = false; //or else it literally disables marios collision
				MarioEvent.s.SetEvent(eEventPl.demoMoon);

				anim.Play("get");

				transform.position = MarioController.s.transform.position;
				transform.rotation = MarioController.s.transform.rotation;

				string t_date = System.DateTime.UtcNow.ToShortDateString(); //even works on 3ds
				scr_main.s.transform.GetChild(1).transform.GetChild(1).GetChild(1).gameObject.GetComponent<Text>().text = moonName;
				scr_main.s.transform.GetChild(1).transform.GetChild(1).GetChild(2).gameObject.GetComponent<Text>().text = t_date;

				break;
		}
	}

	void Update()
	{
		switch (currentState)
		{
			case 0://normal rotate
				if (rotateAddition > 0)
				{
					rotateAddition -= 15f;
				}
				transform.Rotate(0, (-150f + -rotateAddition) * Time.deltaTime, 0);
				break;
			case 1://collected
				if (!scr_manAudio.s.isPlaying(false))
				{
					MarioEvent.s.SetEvent(eEventPl.demoMoon, 2);
					Destroy(gameObject);
				}
				break;
		}
	}
}