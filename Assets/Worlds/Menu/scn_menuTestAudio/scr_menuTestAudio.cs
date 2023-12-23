using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class scr_menuTestAudio : MonoBehaviour {

	int i = 0;
	Text text;
	void Start () {
		text = transform.GetChild(0).GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
			if (i == 0) i = scr_manAudio.s.listSound.Length;
            i--;
			UpdateText();
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
		{
			if (i == scr_manAudio.s.listSound.Length) i = 0;
			i++;
			UpdateText();
		}
		if (Input.GetKeyDown(KeyCode.Return) || UnityEngine.N3DS.GamePad.GetButtonTrigger(N3dsButton.A))
			scr_manAudio.s.PlaySND((eSnd)i);
	}
	void UpdateText()
    {
		text.text = scr_manAudio.s.listSound[i].name;

	}
}
