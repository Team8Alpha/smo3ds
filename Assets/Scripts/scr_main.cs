using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class scr_main : MonoBehaviour {

	// Initialization script for Super Mario Odyssey for 3ds, made by Team Alpha.
	public string version = "0.4";
	public bool isRelease = false; //change via Assets/Resources/Objects/objGlobal<scr_main>
	#define testingBool = true;
	//constants
	[HideInInspector] public static scr_main _f;
	[HideInInspector] public static GUIStyle stl_debug; //style for debug menu
	[HideInInspector] public static LayerMask lyr_enemy;
	[HideInInspector] public static LayerMask lyr_obj; //TODO: rewrite cappy catch sorting system
	[HideInInspector] public static LayerMask lyr_def;
	[HideInInspector] public static LayerMask lyr_player;
	//modifiable
	[HideInInspector] public bool isFocused = true; //objects test if this is true, to run.
	[HideInInspector] public int dbg_enemyCount = 0; 
	[HideInInspector] public int coinsCount = 0;
	[HideInInspector] public int moonsCount = 0;
	[HideInInspector] public int nextSpawn = -1; // next index of spawn point
	[HideInInspector] public int lastCheckpoint = 0; //TODO: write true checkpoint area code, and use this smh.(used by data saving)
	[HideInInspector] public string capMountPoint = "missingno"; //used by cap
	[HideInInspector] public bool hasLevelLoaded = false; //used by level loading and data.
	
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
		lyr_obj = LayerMask.NameToLayer ("Object"); //TODO: rewrite cappy catch sorting with paramObj
		lyr_def= LayerMask.NameToLayer ("Default");
		lyr_player = LayerMask.NameToLayer ("Player");

		string authorVersion;
		if (isRelease)
			authorVersion = "SMO3DS a" + version; //TODO: maybe make this some compiling #if, to save resources at runtime?
		else 
			authorVersion = "SMO3DS pre-a" + version;
		transform.GetChild (1).GetChild (0).GetComponent<Text> ().text = authorVersion;
	}
	public void SetCMD(string text, bool isEditorOut = true){
		if (isRelease)
			return;
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
