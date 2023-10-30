using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_manageInput : MonoBehaviour {

	public static Vector2 AxisDpad(bool rotateCam = false){
		Vector2 dpad = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

		if(rotateCam){
			// Rotate the dpad around the y-axis of the camera
			Vector3 dpad3D = new Vector3(dpad.x, 0f, dpad.y);
			dpad3D = Quaternion.Euler(0f, MarioCam.marioCamera.transform.eulerAngles.y, 0f) * dpad3D;
			dpad = new Vector2(dpad3D.x, dpad3D.z);
		}
		return dpad;
	}

	public static Vector2 AxisCircleL(bool rotateCam = false){
		Vector2 circleL;
		#if UNITY_EDITOR
		circleL = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
		#else
		circleL = UnityEngine.N3DS.GamePad.CirclePad;
		#endif

		if(rotateCam){
			// Rotate the circleL around the y-axis of the camera
			Vector3 circleL3D = new Vector3(-circleL.x, 0f, circleL.y);
			circleL3D = Quaternion.Euler(0f, -MarioCam.marioCamera.transform.eulerAngles.y, 0f) * circleL3D;
			circleL = new Vector2(circleL3D.x, circleL3D.z);
		}
		return circleL;
	}
	public static Vector2 AxisDir(int num){
		Vector2 nums = new Vector2(0, num);

		// Rotate the circleL around the y-axis of the camera
		Vector3 circleL3D = new Vector3(-nums.x, 0f, nums.y);
		circleL3D = Quaternion.Euler (0f, -MarioCam.marioCamera.transform.eulerAngles.y, 0f) * circleL3D;
		nums = new Vector2(circleL3D.x, circleL3D.z);
		return nums;
	}

	public static Vector2 AxisCircleR(){ //x, y
		if(UnityEngine.N3DS.GamePad.IsCirclePadProConnected()) return UnityEngine.N3DS.GamePad.CirclePadPro; else return AxisDpad(false);
	}
}
