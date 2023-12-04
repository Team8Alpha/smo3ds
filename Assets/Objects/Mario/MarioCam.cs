using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MarioCam : MonoBehaviour {

	private GameObject player; // The Player object to follow
	public Transform target; // The parent object for the camera
	public Transform actualCamera; // The actual camera child object

	public bool isInvertCursorY 							= false; // Whether to invert the Y axis of the cursor movement
	public bool isInvertCursorX							= false; // Whether to invert the Y axis of the cursor movement
	public float cursorSensitivity 						= 10f; // The sensitivity of the cursor movement
	public float CStickSensitivity 						= 10f; // The sensitivity of the C-Stick movement
	//public bool isThirdPerson 						= true; // Whether the camera is in third-person view
	public bool isLocked 								= false; //if cam is locked.

	private float cursorX, cursorY; // The current cursor position
	private Vector3 velocity 							= Vector3.zero; // The current velocity of the camera
	private float cameraRotation 						= 0; // Holds mario's current cam rotation.
	private Vector3 cameraControl; // Variable for controlled camera offset(cstick)
	private float targetedY; //position camera wants to move to.
	private Quaternion targetRot; //rotation smoothing
	private Vector3 targetPos = Vector3.zero; //position smoothing
	private float targetCamDistance 					= 0;

	public float confStickXmax 							= 1;// * stick X movement
	public float confStickYmax 							= 1;//same Y
	public bool confRotate 								= true; // if false, then dont rotate.
	public bool confSmooth 								= false; // if false, then dont smooth
	public bool confSmoothY 							= true; // if true, all rotations are smoothed out.(normally useless, only for cutscenes)
	public bool confWalk 								= true; // if false, dont follow walk
	public float confRotateSpeed 						= 0.1f;
	public float confSmoothTime 						= 1f; // The duration of the camera smoothing
	public float confCamDistance 						= 13f; // The distance between the camera and the target
	public float confYOffset 							= 3;

	//default values
	[HideInInspector] public float defRotateSpeed 		= 0.1f;
	[HideInInspector] public float defSmoothTime 		= 0.3f; // The duration of the camera smoothing
	[HideInInspector] public float defCamDistance 		= 5f; // The distance between the camera and the target
	[HideInInspector] public float defYOffset 			= 3;

	public static MarioCam marioCamera;

	void Awake() {
		target = transform; // Set the camera parent to the script's transform
		player = MarioController.marioObject.gameObject;
		actualCamera = target.GetChild(0); // Assuming the camera is the first child of target
		cursorX = 0.0f; cursorY = 0.0f;
		Cursor.lockState = CursorLockMode.Locked; // Lock the cursor to the center of the screen
		Cursor.visible = false; // Hide the cursor

		defRotateSpeed = confRotateSpeed;
		defSmoothTime = confSmoothTime;
		defCamDistance = confCamDistance;
		defYOffset = confYOffset;

		transform.localRotation = Quaternion.Euler (transform.localEulerAngles.y, 0, transform.localEulerAngles.z);
		targetedY = MarioController.marioObject.transform.position.y;

		marioCamera = this;
	}

	void Update() {
		
		if (!isLocked) {
			float cursorXtemp=0;
			float cursorYtemp=0;
			#if UNITY_EDITOR
				cursorXtemp = ((Input.GetAxis ("Mouse X") * cursorSensitivity * Time.deltaTime));
				cursorYtemp = ((Input.GetAxis ("Mouse Y") * cursorSensitivity * Time.deltaTime));
			#else
			if(UnityEngine.N3DS.GamePad.GetButtonHold(N3dsButton.L)) 
			{
				cursorXtemp = ((UnityEngine.N3DS.GamePad.CirclePad.x * cursorSensitivity * Time.deltaTime));
				cursorYtemp = ((UnityEngine.N3DS.GamePad.CirclePad.y * cursorSensitivity * Time.deltaTime));
			} else if(UnityEngine.N3DS.GamePad.IsCirclePadProConnected())
			{
				cursorXtemp = ((UnityEngine.N3DS.GamePad.CirclePadPro.x * cursorSensitivity * Time.deltaTime));
				cursorYtemp = ((UnityEngine.N3DS.GamePad.CirclePadPro.y * cursorSensitivity * Time.deltaTime));
				if(cursorXtemp > 0f) cursorXtemp = 1; if(cursorYtemp > 0f) cursorYtemp = 1; //roughen camera sensitivity
				if(cursorXtemp < 0f) cursorXtemp = -1; if(cursorYtemp < 0f) cursorYtemp = -1; //roughen camera sensitivity
			}
			#endif
			if (isInvertCursorX)
				cursorXtemp = -cursorXtemp;
			if (isInvertCursorY)
				cursorYtemp = -cursorYtemp;
			
			cursorY += cursorYtemp;
			cursorX += cursorXtemp;

			cursorY = Mathf.Clamp (cursorY, -180, 180); // Clamp the Y axis to prevent camera flipping

			cameraControl = new Vector2 (cursorY / 2.2f, cursorX);
			if (confRotate && MarioController.marioObject.isMoving) {
				float targetCameraRotation = player.transform.eulerAngles.y - target.eulerAngles.y;

				if (targetCameraRotation > 180)
					targetCameraRotation -= 360;
				if (targetCameraRotation < -180)
					targetCameraRotation += 360; //idk it suddenly changed numbers so i backfired and it worked.
				if (targetCameraRotation > 170 || targetCameraRotation < -170)
					targetCameraRotation = 0;

				// Smoothly interpolate towards the target camera rotation
				cameraRotation = Mathf.LerpAngle (cameraRotation, cameraRotation + targetCameraRotation, Time.unscaledDeltaTime * confRotateSpeed);
			}
			targetRot = Quaternion.Euler (target.localRotation.x + cameraControl.x, cameraRotation + cameraControl.y, target.localRotation.z + cameraControl.y * 2);
			if (!confSmooth) {
				target.localRotation = targetRot;
				//rotation controls
			}
			actualCamera.transform.localPosition = new Vector3 (0, 0, -targetCamDistance);
			//actual camera offset

			if (confWalk) {
				target.position = new Vector3 (player.transform.position.x, targetedY, player.transform.position.z);
				//move camera with player									//smoothly calculate y position

				RaycastHit hit;
				if (Physics.Raycast (target.transform.position, actualCamera.transform.position - target.position, out hit, confCamDistance) && hit.collider.tag == "camBlock") {
					actualCamera.transform.position = hit.point + hit.normal * 0.2f; // Don't peek into walls!
				}
				actualCamera.LookAt (target.transform); // Look at the camera target
			}
			if (confSmoothY) {
				targetedY = Mathf.SmoothStep (targetedY, MarioController.marioObject.groundedPosition + confYOffset, confSmoothTime);
			} else if (confSmooth) {
				target.localRotation = Quaternion.Slerp (target.localRotation, targetRot, 1 - Time.unscaledTime * confSmoothTime);
				target.position = Vector3.Lerp (target.position, targetPos, Time.unscaledTime * confSmoothTime);
				actualCamera.localRotation = Quaternion.Euler (0, 0, 0);
			} else {
				targetedY = MarioController.marioObject.groundedPosition + confYOffset;
			}
			targetCamDistance = Mathf.SmoothDamp (targetCamDistance, confCamDistance, ref velocity.y, confSmoothTime, Mathf.Infinity, Time.unscaledDeltaTime);
		}
	}
	public void ResetValue(){
		confRotateSpeed = defRotateSpeed;
		confSmoothTime = defSmoothTime;
		confCamDistance = defCamDistance;
		confYOffset = defYOffset;
	}
}