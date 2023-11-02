using UnityEngine;

public class paramObj : MonoBehaviour {

	public bool isTouch = false;
	public bool isCapture = false;
	public bool isHack = true;
	public float bOffsetY = 0f;
	public bool isLOD = true;

	public float bCenterY(){
		return transform.position.y + bOffsetY; 
	}
	void Awake(){
		if (isLOD)
			transform.GetChild(1).gameObject.SetActive (false);
		this.enabled = false;
	}
}
