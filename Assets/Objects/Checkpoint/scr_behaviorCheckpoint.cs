﻿using UnityEngine;

public class scr_behaviorCheckpoint : MonoBehaviour {

	public int numSpawnPoint = 0; //which spawn point is this checkpoint adressed to.
	private bool wasActivated = false;
	[SerializeField] private Material mat_after; 

	Animator anim;
	// Use this for initialization
	void Start() {
		anim = GetComponent<Animator>();
	}

    void OnTouch(int type)
	{

		if (type >= 1)
		{
            if (!wasActivated)
			{
				transform.GetChild(1).GetChild(1).gameObject.GetComponent<Renderer>().material = mat_after;
				scr_manageData.s.Save();
			}
			anim.Play("get");
			wasActivated = true;

			if (type == 1)
			{

			} else
			{
				//MarioController.s.setAnim (" ");
			}
		}
	}
}