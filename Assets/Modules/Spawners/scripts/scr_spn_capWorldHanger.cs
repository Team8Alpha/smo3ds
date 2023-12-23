using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_spn_capWorldHanger : MonoBehaviour {

	public bool isEnabled = false; //for lamps
	public bool isWhite = true;
	public int myType = 0;
	void Awake () {
		GameObject capHang = scr_summon.s.s_object (1 + myType, transform.position, transform.eulerAngles);
		capHang.transform.localScale = capHang.transform.localScale + (transform.localScale-(new Vector3(0.5f,0.5f,0.5f)));
		capHang.GetComponent<scr_behaviorCapHanger> ().isEnabled = isEnabled;
		capHang.GetComponent<scr_behaviorCapHanger> ().isWhite = isWhite;
		Destroy (gameObject);
	}
}
