//#if !isRelease
#if true
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class scr_devMenu: MonoBehaviour {


	private bool isOpen = false;
	private float ypos = 14;
	private float xoffset = 10;
	private int selection = 3;
	private bool noButtonPressed = true;
	private bool submenu = false;
	private int selectionSub = -1;
	private int maxOption = 6;
	private bool deb_fpsIsShowing = true;
	private bool deb_enemyIsShowing = false;
	private bool deb_cmdIsShowing = true;
	private int height = 12;
	private bool canSelect = false;
	static public string txt_cmdOut = "";

	void ResetVal () {
		canSelect = false;
		selection = 3;
		noButtonPressed = true;
		submenu = false;
		selectionSub = -1;
		scr_main._f.SetFocus(true);
	}

	void Update () {
		if (UnityEngine.N3DS.GamePad.GetButtonHold (N3dsButton.L) && UnityEngine.N3DS.GamePad.GetButtonHold (N3dsButton.Start) || Input.GetKey(KeyCode.Escape) && Input.GetKey(KeyCode.Return)) {
			isOpen = true;
			maxOption = 6;
			ResetVal ();
			scr_main._f.SetFocus(false);
		} else if (UnityEngine.N3DS.GamePad.GetButtonHold (N3dsButton.R) && UnityEngine.N3DS.GamePad.GetButtonHold (N3dsButton.Start) || Input.GetKey(KeyCode.Escape) && Input.GetKey(KeyCode.LeftShift)) {
			isOpen = false;
			ResetVal ();
			scr_main._f.SetFocus(true);
		}
		if(isOpen){
			if(noButtonPressed){
				if(submenu){
					if(Input.GetKeyDown(KeyCode.UpArrow)) selectionSub--;
					if(Input.GetKeyDown(KeyCode.DownArrow)) selectionSub++;
					if(Input.GetKeyDown(KeyCode.Escape) || UnityEngine.N3DS.GamePad.GetButtonHold(N3dsButton.B)){ submenu = false; selection = 3; selectionSub = 3; maxOption = 6; canSelect = false;}
				} else {
					if(Input.GetKeyDown(KeyCode.UpArrow)) selection--;
					if(Input.GetKeyDown(KeyCode.DownArrow)) selection++;
					if(Input.GetKeyDown(KeyCode.Escape) || UnityEngine.N3DS.GamePad.GetButtonHold(N3dsButton.B)){ scr_main._f.SetFocus(true); isOpen = false;}
				}
				if(Input.GetKeyDown(KeyCode.Return) || UnityEngine.N3DS.GamePad.GetButtonHold(N3dsButton.A)){
					switch(selection-3){
					case 0:
						isOpen = false;
						ResetVal();
						SceneManager.LoadScene ("scn_menuTitle");
						break;
					case 1:
						submenu = true;
						if (canSelect) {
							isOpen = false;
							string tmpLvName = "ERROR";
							switch (selectionSub - 3) {
							case 0:
								tmpLvName = "scn_test0";
								break;
							case 1:
								tmpLvName = "scn_test1";
								break;
							case 2:
								tmpLvName = "scn_capMain0";
								break;
							case 3:
								tmpLvName = "scn_globShipInside";
								break;
							case 4:
								tmpLvName = "scn_fallsMain0";
								break;
							case 5:
								tmpLvName = "scn_capExTower";
								break;
							}
							ResetVal ();
							scr_loadScene._f.StartScene (tmpLvName, 0);
						}
						maxOption = 8;
						canSelect = true;
						break;
					case 2:
						submenu=true;
						switch(selectionSub-3){
						case 0:
							if(deb_fpsIsShowing) deb_fpsIsShowing=false; else deb_fpsIsShowing=true;
							break;
						case 1:
							if(deb_enemyIsShowing) deb_enemyIsShowing=false; else deb_enemyIsShowing=true;
							break;
						case 2:
							if(deb_cmdIsShowing) deb_cmdIsShowing=false; else deb_cmdIsShowing=true;
							break;
						}
						maxOption = 5;
						break;
					case 3:
						scr_manageData._f.Save ();
						break;
					}
				}
			}
			if(!submenu){
				if(selection < 3) selection = maxOption;
				if(selection > maxOption) selection = 3;
			} else {
				if(selectionSub < 3) selectionSub = maxOption;
				if(selectionSub > maxOption) selectionSub = 3;
			}
#if UNITY_EDITOR
				if (Input.GetKeyDown (KeyCode.UpArrow)
					|| Input.GetKeyDown (KeyCode.DownArrow)
					|| Input.GetKeyDown (KeyCode.Return)
					|| Input.GetKeyDown (KeyCode.Escape))
				noButtonPressed = false;
				else
					noButtonPressed = true;
#else
				if (UnityEngine.N3DS.GamePad.GetButtonHold (N3dsButton.A)
				|| UnityEngine.N3DS.GamePad.GetButtonHold (N3dsButton.B))
				noButtonPressed = false;
				else
				noButtonPressed = true;
#endif
		}
	}

	void DoPrint(int posN = 10, string text = "ERROR", int xOffSet = 15, int _width = 500){
		GUI.Label(new Rect(xOffSet+xoffset, ypos*posN, _width, height), text, scr_main.stl_debug);
	}

	void OnGUI(){
		if(isOpen){
			DoPrint (0, "SMO3DS DEV MENU - TEAM ALPHA", 1);
			DoPrint (1, " ", 1);
			if(submenu){
				switch (selection - 3) {
				case 1:
					DoPrint (selectionSub, " > ", 1, 40);
					DoPrint (2, "Map Select", 1);
					DoPrint (3, "TestMap0-Basic", 15);
					DoPrint (4, "TestMap1-Camera", 15);
					DoPrint (5, "WorldCap-Main", 15);
					DoPrint (6, "Global-InOdyssey", 15);
					DoPrint (7, "WorldFalls-Main0", 15);
					DoPrint (8, "WorldCap-ExTower", 15);
					break;
				case 2:
					DoPrint (selectionSub, " > ", 1, 40);
					DoPrint (2, "Toggle Debug", 1);
					DoPrint (3, "Toggle FPS: " + deb_fpsIsShowing, 15);
					DoPrint (4, "Toggle ENEMY: " + deb_enemyIsShowing, 15);
					DoPrint (5, "Toggle CMD: " + deb_cmdIsShowing, 15);
					break;
				default:
					submenu = false;
					break;
				}
			} else {
				DoPrint (selection, " > ", 1, 40);
				DoPrint (2, "Main Menu", 1);
				DoPrint (3, "Load Title Screen", 15);
				DoPrint (4, "Map Select", 15);
				DoPrint (5, "Toggle Debug", 15);
				DoPrint (6, "Save Game", 15);
			}
		} else {
			if(deb_fpsIsShowing) DoPrint (0, "FPS: "+1/Time.deltaTime, 1, 50);
			if(deb_enemyIsShowing) DoPrint (1, "ENEMY: "+scr_main._f.dbg_enemyCount, 1, 50);
			if(MarioController.marioObject != null){
				DoPrint (4, "COIN: "+scr_main._f.coinsCount, 1, 50);
				DoPrint (5, "MOON: "+scr_main._f.moonsCount, 1, 50);
			}
		}
		if (deb_cmdIsShowing && txt_cmdOut != "")
			DoPrint (16, txt_cmdOut, -5, 1000);
	}
}
		
#endif