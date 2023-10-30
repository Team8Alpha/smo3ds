using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_fadefull : MonoBehaviour {

	public static scr_fadefull _f;
	private bool fadeIn = true;
	private int conf = 0;
	private float fadeSpeed = 0.01f;
	public bool isDone = false;
	private bool isFocusOnExit = false;
	private bool isReverseOnExit = false;
	private bool isKillOnExit = false;
	[SerializeField] private SpriteRenderer colUp;
	[SerializeField] private SpriteRenderer colDown;

	public scr_fadefull Run(bool fadeIn=true, int conf=0, float fadeSpeed = 0.01f, bool isKillOnExit = false, bool isFocusOnExit = false, bool isReverseOnExit = false) {
		isDone = false;
		gameObject.SetActive(true);
		this.fadeIn = fadeIn;
		this.conf = conf;
		this.fadeSpeed = fadeSpeed;
		this.isFocusOnExit = isFocusOnExit;
		this.isReverseOnExit = isReverseOnExit;
		this.isKillOnExit = isKillOnExit;
		switch(conf){
		case 0://full
			colUp.gameObject.SetActive(true);
			colDown.gameObject.SetActive(true);
			break;
		case 1://top
			colUp.gameObject.SetActive(true);
			colDown.gameObject.SetActive(false);
			break;
		case 2://bottom
			colUp.gameObject.SetActive(false);
			colDown.gameObject.SetActive(true);
			break;
		}
		if (fadeIn) {
			colUp.color = new Color (0, 0, 0, 1);
			colDown.color = new Color (0, 0, 0, 1);
		} else {
			colUp.color = new Color (0, 0, 0, 0);
			colDown.color = new Color (0, 0, 0, 0);
		}
		return this;

	}
	void Awake () {
		try{
			if (scr_fadefull._f == null)
				scr_fadefull._f = this;
			else
				Destroy (gameObject);

			} catch(System.Exception e){
				scr_main._f.SetCMD (" " + e);
		}
		if (isDone) {
			colUp.color = new Color (0, 0, 0, 0);
			colDown.color = new Color (0, 0, 0, 0);
			gameObject.SetActive (false);
		}
	}

	void Update () {
		if (!isDone) {
			if (fadeIn) {
				switch (conf) {
				case 0:
					colUp.color = new Color (0, 0, 0, colUp.color.a - fadeSpeed);
					colDown.color = new Color (0, 0, 0, colDown.color.a - fadeSpeed);
					if (colDown.color.a <= 0) {
						isDone = true;
					}
					break;
				case 1:

					colDown.color = new Color (0, 0, 0, colDown.color.a - fadeSpeed);
					if (colDown.color.a <= 0) {
						isDone = true;
					}
					break;
				case 2:

					colUp.color = new Color (0, 0, 0, colUp.color.a - fadeSpeed);
					if (colUp.color.a <= 0) {
						isDone = true;
					}
					break;
				}
			} else {
				switch (conf) {
				case 0:
					colUp.color = new Color (0, 0, 0, colUp.color.a + fadeSpeed);
					colDown.color = new Color (0, 0, 0, colDown.color.a + fadeSpeed);
					if (colDown.color.a >= 1) {
						isDone = true;
					}
					break;
				case 1:
					colDown.color = new Color (0, 0, 0, colDown.color.a + fadeSpeed);
					if (colDown.color.a >= 1) {
						isDone = true;
					}
					break;
				case 2:
					colUp.color = new Color (0, 0, 0, colUp.color.a + fadeSpeed);
					if (colUp.color.a >= 1) {
						isDone = true;
					}
					break;
				}
			}
		} else {
			if (isFocusOnExit)
				scr_main._f.focusOn ();
			if (isReverseOnExit) {
				Run (!fadeIn, conf, fadeSpeed, isFocusOnExit, false);
			} else if (isKillOnExit)
				Destroy (gameObject);
			else if(fadeIn) gameObject.SetActive (false);
		}
	}
}
