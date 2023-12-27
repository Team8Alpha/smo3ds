using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine;
using System.Collections;

public class scr_manButton : MonoBehaviour
{
	public GameObject[] buttons;//resume button
	public Vector2 posOffsetSel = Vector2.zero;
	public GameObject buttonMan;
	public GameObject buttonPrevMan;
	public static scr_manButton s;
	public GameObject iconSelect;//cappy icon

	private int currentButton = 0; //number of current button
	private bool buttonPressed = false; //if it was pressed once, it has to be 0 to be able to be pressed again, so it wont just go party with the buttons.
	private Vector3 iconSelectPos; // set everytime new button selected, this is cuz he moves left and right.

	public void SetMenu(GameObject[] lbuttons, GameObject lbuttonMan, byte firstSelectPos, Vector2 lposOffsetSel)
    {
		if(buttonMan != null) buttonPrevMan = buttonMan;
		buttons = lbuttons;
		buttonMan = lbuttonMan;
		posOffsetSel = lposOffsetSel;
		currentButton = firstSelectPos;
		iconSelect.SetActive(true);
		EventSystem.current.SetSelectedGameObject(buttons[currentButton]);
		setPosition(buttons[currentButton].transform.position);
		this.enabled = true;
	}
	public void SetActive(bool boolean)
    {
		this.enabled = boolean;
        if (boolean)
        {
			EventSystem.current.SetSelectedGameObject(buttons[currentButton]);
			setPosition(buttons[currentButton].transform.position);
		}
		iconSelect.SetActive(boolean);
	}
	public void SwitchPrevMenu()
    {
		buttonPrevMan.SendMessage("OnMenu");
	}

	void setPosition(Vector3 position)
	{
		iconSelect.transform.position = new Vector3(posOffsetSel.x + position.x - buttons[currentButton].GetComponent<RectTransform>().rect.width/2, posOffsetSel.y + position.y, -300);
	}

	void Start()
	{
		iconSelect = transform.GetChild(3).gameObject;
		s = this;
		this.enabled = false;
		SceneManager.sceneLoaded += OnSceneLoaded;
	}
	void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		iconSelect.SetActive(false);
	}

	void Update()
	{
		if (scr_main.s.isFocused)
		{
			float h = UnityEngine.N3DS.GamePad.CirclePad.y + Input.GetAxisRaw("Vertical");
			if (!buttonPressed && h != 0)
			{
				buttonPressed = true;
				if (h > 0)
					if (currentButton <= 0)
						currentButton = buttons.Length - 1; //HERE MAXIMAL BUTTON NUMBER
					else
						currentButton--;
				if (h < 0)
					if (currentButton >= buttons.Length - 1) //HERE MAXIMAL BUTTON NUMBER
						currentButton = 0;
					else
						currentButton++;
				EventSystem.current.SetSelectedGameObject(buttons[currentButton]);
				setPosition(buttons[currentButton].transform.position);
			}
			else if (h == 0)
				buttonPressed = false;

			if (UnityEngine.N3DS.GamePad.GetButtonHold(N3dsButton.A) || Input.GetKey(KeyCode.Return))
			{
				buttonMan.SendMessage("OnButtonPress", currentButton);
				iconSelect.SetActive(false);
				this.enabled = false;
			}
		}
	}
}
