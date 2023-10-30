using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_persistent : MonoBehaviour {

	void Awake () {
		DontDestroyOnLoad (gameObject);
		this.enabled = false;
	}
}
