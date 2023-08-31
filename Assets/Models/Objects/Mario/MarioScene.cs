using UnityEngine;
using System.Collections;

public class MarioScene : MonoBehaviour
{
	MarioScene _f;
	MarioController mario;
	// Use this for initialization
	void Start ()
	{
		_f = this;
		mario = MarioController.marioObject;
		this.enabled = false;
	}
	void PlayerDied(int type){
		//need animations for that
	}
}

