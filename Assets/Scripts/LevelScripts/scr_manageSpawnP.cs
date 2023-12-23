using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class scr_manageSpawnP : MonoBehaviour {

	void Update ()
	{
		if (scr_main.s == null)
			SceneManager.LoadScene (SceneManager.GetActiveScene ().name);
		else if (scr_main.s.hasLevelLoaded) {
			if (scr_main.s.nextSpawn == -1)
				scr_main.s.nextSpawn = 0;
			scr_main.DPrint ("SP:" + scr_main.s.nextSpawn);
			Transform spawnPos = transform.GetChild (scr_main.s.nextSpawn).transform;
			scr_summon.s.s_player (spawnPos.position, spawnPos.eulerAngles);
			scr_main.s.nextSpawn = -1;
			this.enabled = false;
		}
	}
}