using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MarioCam : MonoBehaviour {

	private GameObject player; // The Player object to follow
	public Transform target; // The parent object for the camera
	public Transform actualCamera; // The actual camera child object

	public bool invertCursorY = false; // Whether to invert the Y axis of the cursor movement
	public bool invertCursorX = false; // Whether to invert the Y axis of the cursor movement
	public float cameraDistance = 5f; // The distance between the camera and the target
	public float wallOffset = 0.1f; // The offset to move the camera back when it collides with a wall
	public float cursorSensitivity = 10f; // The sensitivity of the cursor movement
	public float CStickSensitivity = 10f; // The sensitivity of the C-Stick movement
	//public bool isThirdPerson = true; // Whether the camera is in third-person view
	public float smoothTime = 0.3f; // The duration of the camera smoothing
	public bool isLocked = false; //if cam is locked.
	public float camYOffset = 3;

	private float cursorX, cursorY; // The current cursor position
	private float cStickX, cStickY; // The current cursor position
	private Vector3 velocity = Vector3.zero; // The current velocity of the camera
	private float cameraRotation = 0; // Holds mario's current cam rotation.
	private Vector3 cameraControl; // Variable for controlled camera offset(cstick)
	private float targetedY; //position camera wants to move to.

	private float confStickXmax = 1;// * stick X movement
	private float confStickYmax = 1;//same Y
	private bool confRotate = true; // if false, then dont rotate.
	private bool confSmooth = true; // if false, then dont smooth
	private bool confWalk = true; // if false, dont follow walk
	private int confMode = 0; // camera mode.
	public float confRotateSpeed = 0.1f;

	public static MarioCam marioCamera;

	public void setState(int state, Vector3 tpos, Quaternion trot) {
		confMode = state;
		transform.rotation = trot;
		transform.localPosition = tpos;
		switch (confMode) {
		case 0: // OW FREE
			confRotate = true;
			confWalk = true;
			confSmooth = true;
			confStickXmax = 1;
			confStickYmax = 1;
			break;
		case 1: // STATIC
			confRotate = false;
			confWalk = false;
			confSmooth = false;
			confStickXmax = 0;
			confStickYmax = 0;
			break;
		case 2: // OW FREE+STATIC PATH
			confRotate = false;
			confWalk = true;
			confSmooth = true;
			confStickXmax = 0;
			confStickYmax = 0;
			break;
		case 3: // OW 2D
			break;
		case 4: // OW HEAD
			break;
		}
	}

	void Awake() {
		target = transform; // Set the camera parent to the script's transform
		player = MarioController.marioObject.gameObject;
		actualCamera = target.GetChild(0); // Assuming the camera is the first child of target
		cStickX = 0.0f; cStickY = 0.0f;
		Cursor.lockState = CursorLockMode.Locked; // Lock the cursor to the center of the screen
		Cursor.visible = false; // Hide the cursor
		marioCamera = this;
	}

	void Update() {

		if (!isLocked) {

			#if UNITY_EDITOR
			cursorX += ((Input.GetAxis ("Mouse X") * cursorSensitivity * Time.deltaTime * -1) * confStickXmax) * (invertCursorX ? -1 : 1);
			cursorY += ((Input.GetAxis ("Mouse Y") * cursorSensitivity * Time.deltaTime * -1) * confStickYmax) * (invertCursorY ? -1 : 1);
			cursorY = Mathf.Clamp (cursorY, -20, 70f); // Clamp the Y axis to prevent camera flipping
			#else
			cStickX += ((UnityEngine.N3DS.GamePad.CirclePadPro.x * cursorSensitivity * Time.deltaTime * -1) * confStickXmax)*(invertCursorX?-1:1);
			cStickY += ((UnityEngine.N3DS.GamePad.CirclePadPro.y * cursorSensitivity * Time.deltaTime * -1) * confStickYmax)*(invertCursorY?-1:1);
			cStickY = Mathf.Clamp(cStickY, -20, 70f); // Clamp the Y axis to prevent camera flipping
			#endif

			cameraControl = new Vector2 ((cursorY + cStickY) / 2.2f, cursorX + cStickX);
			if (confRotate && MarioController.marioObject.isMoving) {
				float targetCameraRotation = player.transform.eulerAngles.y - target.eulerAngles.y;

				if (targetCameraRotation > 180) targetCameraRotation -= 360;
				if (targetCameraRotation < -180) targetCameraRotation += 360; //idk it suddenly changed numbers so i backfired and it worked.
				if(targetCameraRotation > 170 || targetCameraRotation < -170) targetCameraRotation=0;

				// Smoothly interpolate towards the target camera rotation
				cameraRotation = Mathf.LerpAngle (cameraRotation, cameraRotation+targetCameraRotation, Time.deltaTime * confRotateSpeed);
			}
			transform.rotation = Quaternion.Euler (transform.localRotation.x + cameraControl.x, cameraRotation + cameraControl.y, transform.rotation.z + cameraControl.y * 2);
			//rotation controls

			actualCamera.transform.localPosition = new Vector3 (0, 0, -cameraDistance);
			if (confWalk) {
				float smoothSpeed = cameraDistance / 0.5f;
				// ensure it takes half a second to move

				transform.position = new Vector3 (player.transform.position.x, targetedY, player.transform.position.z);
				//move camera with player									//smoothly calculate y position

				RaycastHit hit;
				if (Physics.Raycast (target.transform.position, actualCamera.transform.position - target.transform.position, out hit, cameraDistance) && hit.collider.tag == "camBlock") {
					actualCamera.transform.position = hit.point + hit.normal * 0.2f; // Don't peek into walls!
				}
				actualCamera.LookAt (target.transform); // Look at the camera target
			}
			if (confSmooth)
				targetedY = Mathf.SmoothDamp (target.position.y, MarioController.marioObject.groundedPosition + camYOffset, ref velocity.y, smoothTime);
			else
				targetedY = MarioController.marioObject.groundedPosition + camYOffset;
		}
	}
}