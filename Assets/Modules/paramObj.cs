using UnityEngine;

public class paramObj : MonoBehaviour {

	public bool isTouch = false;
	//public bool isWall = false;
	public bool isCapture = false;
	public bool isHack = true;
	public float bOffsetY = 0f;

	public float bCenterY(){
		return transform.position.y + bOffsetY; 
	}
}
