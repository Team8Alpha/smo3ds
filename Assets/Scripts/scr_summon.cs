using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class scr_summon : MonoBehaviour {
//this script, will have different functions to summon entities. the player script gets its own function tho.

	public static scr_summon f_summon;
	
	public GameObject s_player(Vector3 pos, Vector3 rot){
		if (MarioController.marioObject == null)
        {
			return GameObject.Instantiate(Resources.Load<GameObject>("Objects/objMario"), pos, Quaternion.Euler(rot));
        }
		return null;
	}
	public GameObject s_entity(int type, Vector3 pos, Vector3 rot){
		string myObjString="INVALID";
		switch(type){
		case 0: //goomba
			myObjString = "Objects/objGoomba";
			break;
		}
		GameObject result = GameObject.Instantiate(Resources.Load<GameObject>(myObjString), pos, Quaternion.Euler(rot));
		return result;
	}
	
	public GameObject s_object(int type, Vector3 pos, Vector3 rot){
		string myObjString="INVALID";
		switch (type) {
		case 0: //goomba
			myObjString = "Objects/objCoin";
			break;
		case 1: //capWorldHanger0
			myObjString = "Objects/objCapWorldHanger0";
			break;
		case 2: //capWorldHanger1
			myObjString = "Objects/objCapWorldHanger1";
			break;
		case 3: //capWorldHanger2
			myObjString = "Objects/objCapWorldHanger2";
			break;
		case 4: //capWorldHanger3
			myObjString = "Objects/objCapWorldHanger3";
			break;
		case 5: //capWorldHanger4
			myObjString = "Objects/objCapWorldHanger4";
			break;
		case 6: //moon
			myObjString = "Objects/objMoon";
			break;
		case 7: //blockQuestion
			myObjString = "Objects/objBlockQuestion";
			break;
		case 8: //blockBrick
			myObjString = "Objects/objBlockBrick";
			break;
		case 9: //blockVoid
			myObjString = "Objects/objBlockVoid";
			break;
		case 10: //Stake / brown fat pole
			myObjString = "Objects/objStake";
			break;
		case 11: //Stake / brown fat pole
			myObjString = "Objects/objFrailBox";
			break;
		}
		GameObject result = GameObject.Instantiate(Resources.Load<GameObject>(myObjString), pos, Quaternion.Euler(rot));
		return result;
	}
	
	// Use this for initialization
	[RuntimeInitializeOnLoadMethod]
	void Awake () {
		f_summon = this;
		this.enabled = false;
	}
}
