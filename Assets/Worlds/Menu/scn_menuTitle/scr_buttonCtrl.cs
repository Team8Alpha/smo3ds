using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine;
using System.Collections;

public class scr_buttonCtrl : MonoBehaviour {
	public GameObject buttonRes;//resume button
	public GameObject buttonNew;//new game button
	public GameObject buttonOpt;//options button
	public GameObject iconSelect;//cappy icon
	public float iconOffset = 82; //how much to shift to left, since it would center in button.

	private int currentButton = 0; //number of current button
	private bool buttonPressed = false; //if it was pressed once, it has to be 0 to be able to be pressed again, so it wont just go party with the buttons.
	private Vector3 iconSelectPos; // set everytime new button selected, this is cuz he moves left and right.

	void setPosition(Vector3 position){
		iconSelect.transform.position = new Vector3 (position.x - iconOffset, position.y - 6, -300);
	}

	void Start () {
		setPosition(buttonRes.transform.position);
	}

	void Update () {
		if (scr_gameInit.globalValues.isFocused) {
			float h = UnityEngine.N3DS.GamePad.CirclePad.y + Input.GetAxisRaw ("Vertical");
			if (!buttonPressed && h != 0) {
				buttonPressed = true;
				if (h > 0)
				if (currentButton <= 0)
					currentButton = 1; //HERE MAXIMAL BUTTON NUMBER
				else
					currentButton--;
				if (h < 0)
				if (currentButton >= 1) //HERE MAXIMAL BUTTON NUMBER
					currentButton = 0;
				else
					currentButton++;
				switch (currentButton) { //switch statement, :D
				case 0:
					EventSystem.current.SetSelectedGameObject (buttonRes);
					setPosition (buttonRes.transform.position);
					break;
				case 1:
					EventSystem.current.SetSelectedGameObject (buttonNew);
					setPosition (buttonNew.transform.position);
					break;
				case 2:
					EventSystem.current.SetSelectedGameObject (buttonOpt);
					setPosition (buttonOpt.transform.position);
					break;
				}
			} else if (h == 0)
				buttonPressed = false;
			
			if (UnityEngine.N3DS.GamePad.GetButtonHold (N3dsButton.A) || Input.GetKey (KeyCode.Return)) {
				switch (currentButton) {
				case 0:
					LoadScene ("scn_capMain0");
					break;
				case 1:
					LoadScene ("scn_test0");
					break;
				case 2:
					LoadOptions ();
					break;
				}
			}
		}
	}
	public void LoadOptions()
	{
		Debug.Log ("Wait, a bit, wont ya?");
	}
	void LoadScene(string name){
		StartCoroutine (scr_title._f.ChangeEngineColour());
		if (scr_loadScene._f.nextScene == name)
			return;
		scr_loadScene._f.nextScene = name;
		scr_gameInit.globalValues.focusOff();
		scr_gameInit.globalValues.dbg_enemyCount=0;
		SceneManager.LoadScene ("scn_loadShip", LoadSceneMode.Additive);
		scr_title._f.GetComponent<Animator>().Play ("titleEnd");
		transform.GetComponent<Canvas> ().enabled = false;
		transform.GetChild (0).gameObject.SetActive (false);
		this.enabled = false;
	}
}
