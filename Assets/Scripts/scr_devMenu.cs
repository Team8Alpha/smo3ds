#if !isRelease
//#if true
using System.Collections;
using System.Collections.Generic;
using System;
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
	private int maxOption = 7;
	private bool deb_fpsIsShowing = true;
	private bool deb_enemyIsShowing = false;
	private bool deb_cmdIsShowing = true;
	private bool deb_statsIsShowing = true;
	private int height = 12;
	private bool canSelect = false;
	static public string txt_cmdOut = "";
	private bool hasOpened = false;

	void ResetVal () {
		canSelect = false;
		selection = 3;
		noButtonPressed = true;
		submenu = false;
		selectionSub = -1;
		hasOpened = false;
		scr_main.s.SetFocus(true);
	}

	void Update() {
#if UNITY_EDITOR
		bool buttonBack = Input.GetKey(KeyCode.Escape);
		bool buttonOK = Input.GetKey(KeyCode.Return);
		if (buttonBack && buttonOK)
#else
		bool buttonBack = UnityEngine.N3DS.GamePad.GetButtonHold(N3dsButton.B);
		bool buttonOK = UnityEngine.N3DS.GamePad.GetButtonHold(N3dsButton.A);
		if (UnityEngine.N3DS.GamePad.GetButtonHold(N3dsButton.L) && UnityEngine.N3DS.GamePad.GetButtonHold(N3dsButton.Start))
#endif
		{
			isOpen = true;
			maxOption = 7;
			ResetVal ();
			scr_main.s.SetFocus(false);
		}
		else if
#if UNITY_EDITOR
		(buttonBack && Input.GetKey(KeyCode.LeftShift))
#else

		(UnityEngine.N3DS.GamePad.GetButtonHold (N3dsButton.R) && UnityEngine.N3DS.GamePad.GetButtonHold (N3dsButton.Start))
#endif
		{
			isOpen = false;
			ResetVal ();
			scr_main.s.SetFocus(true);
		}
		if(isOpen){
			if(noButtonPressed && hasOpened){
				if(submenu){
					if(Input.GetKeyDown(KeyCode.UpArrow)) selectionSub--;
					if(Input.GetKeyDown(KeyCode.DownArrow)) selectionSub++;
					if(Input.GetKeyDown(KeyCode.Escape) || buttonBack) { submenu = false; selection = 3; selectionSub = 3; maxOption = 7; canSelect = false;}
				} else {
					if(Input.GetKeyDown(KeyCode.UpArrow)) selection--;
					if(Input.GetKeyDown(KeyCode.DownArrow)) selection++;
					if(Input.GetKeyDown(KeyCode.Escape) || buttonBack) { scr_main.s.SetFocus(true); isOpen = false;}
				}
				if(Input.GetKeyDown(KeyCode.Return) || buttonOK)
				{
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
							case 5:
								tmpLvName = "scn_capExTower";
								break;
							}
							ResetVal ();
							scr_loadScene.s.StartScene (tmpLvName, 0);
						}
						maxOption = 7;
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
							if (deb_cmdIsShowing) deb_cmdIsShowing = false; else deb_cmdIsShowing = true;
							break;
						case 3:
							if (deb_statsIsShowing) deb_statsIsShowing = false; else deb_statsIsShowing = true;
							break;
						}
						maxOption = 6;
						break;
					case 3:
						isOpen = false;
						ResetVal();
						scr_loadScene.s.StartScene("scn_menuTestAudio");
						break;
					case 4:
						scr_manageData.s.Save ();
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
			if (Input.GetKeyDown(KeyCode.UpArrow)
				|| Input.GetKeyDown(KeyCode.DownArrow)
				|| Input.GetKeyDown(KeyCode.Return)
				|| Input.GetKeyDown(KeyCode.Escape))
#else
			if (UnityEngine.N3DS.GamePad.GetButtonHold (N3dsButton.A)
				|| UnityEngine.N3DS.GamePad.GetButtonHold (N3dsButton.B))
#endif
				noButtonPressed = false;
			else
			{
				noButtonPressed = true;
				hasOpened = true;
			}
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
					DoPrint (7, "WorldCap-ExTower", 15);
					break;
				case 2:
					DoPrint (selectionSub, " > ", 1, 40);
					DoPrint (2, "Toggle Debug", 1);
					DoPrint (3, "Toggle FPS: " + deb_fpsIsShowing, 15);
					DoPrint (4, "Toggle ENEMY: " + deb_enemyIsShowing, 15);
					DoPrint (5, "Toggle CMD: " + deb_cmdIsShowing, 15);
					DoPrint (6, "Toggle STATS: " + deb_statsIsShowing, 15);
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
				DoPrint (5, "Debug Settings", 15);
				DoPrint (6, "Enter Sound Test", 15);
				DoPrint (7, "Save Game", 15);
			}
		} else {
			if(deb_fpsIsShowing) DoPrint (0, "FPS: "+1/Time.deltaTime, 1, 50);
			if(deb_enemyIsShowing) DoPrint (3, "ENEMY: "+scr_main.s.dbg_enemyCount, 1, 50);
			if(MarioController.s != null){
				DoPrint (4, "COIN: "+scr_main.s.coinsCount, 1, 50);
				DoPrint (5, "MOON: "+scr_main.s.moonsCount, 1, 50);
			}
#if !UNITY_EDITOR
			if (deb_statsIsShowing){
				DoPrint(1, "CPU: " + UnityEngine.N3DS.Debug.GetSystemFree(), 1, 50);
				DoPrint(2, "RAM: A " + UnityEngine.N3DS.Debug.GetVRAMAFree() + ", B "+UnityEngine.N3DS.Debug.GetVRAMBFree(), 1, 50);
			}
#endif
		}
		if (deb_cmdIsShowing && txt_cmdOut != "")
			DoPrint (16, txt_cmdOut, -5, 1000);
	}
}
		
#endif