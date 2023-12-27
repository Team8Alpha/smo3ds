using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine;
using System.Collections;

public class scrMenuTitle : MonoBehaviour {
	public GameObject[] buttons;

	void Start()
    {
		OnMenu();
		scr_manButton.s.SetActive(false);
    }
	public void OnMenu()
	{
		scr_manButton.s.SetMenu(buttons, gameObject, 0, new Vector2(40, -6));
	}

	public void OnButtonPress(int button)
	{
		switch (button)
		{
			case 0:
				LoadScene("scn_capMain0");
				break;
			case 1:
				LoadScene("scn_test0");
				break;
			case 2:
				SceneManager.LoadScene("scn_menuConfig", LoadSceneMode.Additive);
				break;
		}
	}
	void LoadScene(string name){
		StartCoroutine (scr_title.s.ChangeEngineColour());
		if (scr_loadScene.s.nextScene == name)
			return;
		scr_loadScene.s.nextScene = name;
		scr_main.s.dbg_enemyCount=0;
		SceneManager.LoadScene ("scn_loadShip", LoadSceneMode.Additive);
		scr_title.s.GetComponent<Animator>().Play ("titleEnd");
		transform.GetComponent<Canvas> ().enabled = false;
	}
}
