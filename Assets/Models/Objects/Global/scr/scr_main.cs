using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_main : MonoBehaviour {

	// Initialization script for Super Mario Odyssey for 3ds, made by Team Alpha.

	//constants
	public static scr_main _f;
	public static GUIStyle stl_debug;
	public static LayerMask lyr_enemy;
	public static LayerMask lyr_obj;
	public static LayerMask lyr_def;
	public static LayerMask lyr_player;
	//modifiable
	public bool isFocused = true;
	public int dbg_enemyCount = 0;
	public int coinsCount = 0;
	public int moonsCount = 0;
	public int nextSpawn = -1; // next index of spawn point
	public int lastCheckpoint = 0;
	public string capMountPoint = "missingno";
	public bool hasLevelLoaded = false;
	
	public void focusOff(){
		isFocused = false;
		Time.timeScale=0;
	}
	public void focusOn(){
		isFocused = true;
		Time.timeScale=1;
	}
	
	void Awake(){
		if(_f == null)
		{
			Debug.ClearDeveloperConsole ();
			SetCMD("INITIALIZE SUPER MARIO ODYSSEY - 3DS DEMAKE");
			_f = this;
			DontDestroyOnLoad(gameObject);
			Init ();
			return;
		}
		Destroy(gameObject);
	}
	
	void Init () {
		Application.targetFrameRate = 50;
		QualitySettings.vSyncCount = 0;
		//UnityEngine.N3DS.HomeButton.Enable ();

		stl_debug = new GUIStyle();
		stl_debug.normal = new GUIStyleState();
		stl_debug.normal.textColor = Color.white;

		lyr_enemy = LayerMask.NameToLayer ("Enemy");
		lyr_obj = LayerMask.NameToLayer ("Object");
		lyr_def= LayerMask.NameToLayer ("Default");
		lyr_player = LayerMask.NameToLayer ("Player");
	}
	public void SetCMD(string text, bool isEditorOut = true){
		if(scr_devMenu.txt_cmdOut != null) scr_devMenu.txt_cmdOut = text;
		if(isEditorOut && text != "") Debug.Log (text);
	}
	/*void OnApplicationPause(bool paused)
	{
		if (paused)
		{
			scr_manageData._f.Save ();
		}
	}*/
}
