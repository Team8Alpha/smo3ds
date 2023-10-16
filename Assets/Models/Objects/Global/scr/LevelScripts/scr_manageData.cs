using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.SceneManagement;
using System.IO.Compression;

[Serializable]
public class LevelSaveData
{
	public int lastSpawn = 0;
}

[Serializable]
public class SaveData
{
	public int coinsCount = 0;
	public int moonsCount = 0;
	public List<LevelSaveData> levelData;

	public SaveData()
	{
		int defaultMaxLevels = SceneManager.sceneCountInBuildSettings; // or any other default value you prefer
		levelData = new List<LevelSaveData>(defaultMaxLevels);
		for (int i = 0; i < defaultMaxLevels; i++)
		{
			levelData.Add(null);
		}
	}
}
public class scr_manageData : MonoBehaviour
{
	public static scr_manageData _f;

	private const string SAVE_FILE_NAME = "save.json";
	private scr_gameInit globVar;

	void Awake()
	{
		
		//using (var fileStream = new FileStream(Path.Combine (Application.persistentDataPath, "playpref.dat"), FileMode.Create));
		//TEMPORARY
		
		
		_f = this;
		globVar = scr_gameInit.globalValues;
	}

	public void Save()
	{
		scr_gameInit.globalValues.focusOff ();
		scr_gameInit.globalValues.transform.GetChild (2).GetChild (1).gameObject.SetActive (true);
		PrintLog ("N: saving data");
		string filePath = Path.Combine (Application.persistentDataPath, SAVE_FILE_NAME);
		try {
			SaveData saveData;
			int buildIndex = SceneManager.GetActiveScene ().buildIndex;
			if (File.Exists (Path.Combine (Application.persistentDataPath, SAVE_FILE_NAME))) {
				saveData = LoadManual ();
				if (saveData.levelData [buildIndex] == null) {
					Debug.Log ("N: creating saveData for level");
					saveData.levelData [buildIndex] = new LevelSaveData ();
				}
			} else {
				saveData = new SaveData ();
				saveData.levelData [buildIndex] = new LevelSaveData ();
				using (var fileStream = new FileStream(filePath, FileMode.Create));
				PrintLog("N: create save file");
			}

			saveData.coinsCount = globVar.coinsCount;
			saveData.moonsCount = globVar.moonsCount;

			LevelSaveData lvlSave = saveData.levelData[SceneManager.GetActiveScene().buildIndex];//load
			lvlSave.lastSpawn = globVar.lastCheckpoint;//overwrite
			saveData.levelData[SceneManager.GetActiveScene().buildIndex] = lvlSave;//write

			string json = JsonUtility.ToJson(saveData);
			File.WriteAllText(filePath, json);
			PrintLog("N: data saved");
		}
		catch (Exception e)
		{
			PrintLog("Error saving data: {e.Message}");
		}

		scr_gameInit.globalValues.transform.GetChild (2).GetChild (1).gameObject.SetActive (false);
		scr_gameInit.globalValues.focusOn ();
	}


	public bool Load()
	{
		try {
			int buildIndex = SceneManager.GetActiveScene ().buildIndex;
			if (globVar == null)
				return false;
			else if (!globVar.hasLevelLoaded) {
				globVar.hasLevelLoaded = true;
				SaveData data = LoadManual ();
				if (data != null) {
					if (data.levelData == null) {
						PrintLog ("N: create saveData 2");
						data = new SaveData ();
					} else {
						PrintLog ("N: load saveData");
					}
				} else {
					PrintLog ("N: create saveData");
					data = new SaveData ();
					data.levelData [buildIndex] = new LevelSaveData ();
				}
				globVar.coinsCount = data.coinsCount;
				globVar.moonsCount = data.moonsCount;
			}
		} catch (Exception e) {
			PrintLog ("Error in Load: {e.Message}");
			return false;
		}
		return true;
	}

	public void LoadLevel()
	{
		try {
			int buildIndex = SceneManager.GetActiveScene ().buildIndex;
			if (globVar == null)
				return;
			else if (!globVar.hasLevelLoaded) {
				globVar.hasLevelLoaded = true;
				SaveData data = LoadManual ();
				if (data != null) {
					if (data.levelData [buildIndex] != null) {
						PrintLog ("N: load levelData");
					} else {
						PrintLog ("N: create levelData");
						data.levelData.Add(new LevelSaveData());
					}
				} else {
					PrintLog ("N: create saveData");
					data = new SaveData ();
					data.levelData [buildIndex] = new LevelSaveData ();
				}
				globVar.lastCheckpoint = data.levelData [buildIndex].lastSpawn;
				if(scr_gameInit.globalValues.nextSpawn == -1) globVar.nextSpawn = globVar.lastCheckpoint;
			}
		} catch (UnauthorizedAccessException e) {
			PrintLog ("Error in LoadLevel: {e.Message}");
		}
	}

	public void PrintLog(string message)
	{
		Debug.Log(message);
	}

	public SaveData LoadManual()
	{
		SaveData data = new SaveData ();
		try
		{
			if (!File.Exists(Path.Combine(Application.persistentDataPath, SAVE_FILE_NAME)))
			{
				Save();
			}

			string json = File.ReadAllText(Path.Combine(Application.persistentDataPath, SAVE_FILE_NAME));
			SaveData saveData = JsonUtility.FromJson<SaveData>(json);
			return saveData;
		}
		catch (Exception e)
		{
			PrintLog("Error loading save data: {e.Message}");
		}
		return data;
	}
}