using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class scr_manageSpawnP : MonoBehaviour {

	void Update ()
	{
		if (scr_gameInit.globalValues == null)
			SceneManager.LoadScene (SceneManager.GetActiveScene ().name);
		else if (scr_gameInit.globalValues.hasLevelLoaded) {
			if (scr_gameInit.globalValues.nextSpawn == -1)
				scr_gameInit.globalValues.nextSpawn = 0;
			Debug.Log ("SP:" + scr_gameInit.globalValues.nextSpawn);
			Transform spawnPos = transform.GetChild (scr_gameInit.globalValues.nextSpawn).transform;
			scr_summon.f_summon.s_player (spawnPos.position, spawnPos.eulerAngles);
			scr_gameInit.globalValues.nextSpawn = -1;
			this.enabled = false;
		}
	}
}