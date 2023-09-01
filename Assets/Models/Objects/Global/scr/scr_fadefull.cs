using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_fadefull : MonoBehaviour {

	public static scr_fadefull _f;
	private bool fadeIn = true;
	private int conf = 0;
	private float fadeSpeed = 0.01f;
	public bool isDone = false;
	private SpriteRenderer colUp;
	private SpriteRenderer colDown;

	public scr_fadefull Run(bool fadeIn=true, int conf=0, float fadeSpeed = 0.01f) {
		isDone = false;
		gameObject.SetActive(true);
		this.fadeIn = fadeIn;
		this.conf = conf;
		this.fadeSpeed = fadeSpeed;
		colUp = transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>();
		colDown = transform.GetChild(3).gameObject.GetComponent<SpriteRenderer>();
		switch(conf){
		case 0://full
			transform.GetChild(0).gameObject.SetActive(true);
			transform.GetChild(1).gameObject.SetActive(true);
			transform.GetChild(2).gameObject.SetActive(true);
			transform.GetChild(3).gameObject.SetActive(true);
			break;
		case 1://top
			transform.GetChild(0).gameObject.SetActive(true);
			transform.GetChild(1).gameObject.SetActive(true);
			transform.GetChild(2).gameObject.SetActive(false);
			transform.GetChild(3).gameObject.SetActive(false);
			break;
		case 2://bottom
			transform.GetChild(0).gameObject.SetActive(false);
			transform.GetChild(1).gameObject.SetActive(false);
			transform.GetChild(2).gameObject.SetActive(true);
			transform.GetChild(3).gameObject.SetActive(true);
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
		_f = this;
		gameObject.SetActive (false);
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
						gameObject.SetActive (false);
					}
					break;
				case 1:

					colDown.color = new Color (0, 0, 0, colDown.color.a - fadeSpeed);
					if (colDown.color.a <= 0) {
						isDone = true;
						gameObject.SetActive (false);
					}
					break;
				case 2:

					colUp.color = new Color (0, 0, 0, colUp.color.a - fadeSpeed);
					if (colUp.color.a <= 0) {
						isDone = true;
						gameObject.SetActive (false);
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
		}
	}
}
