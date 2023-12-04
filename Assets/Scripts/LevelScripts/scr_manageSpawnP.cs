using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class scr_manageSpawnP : MonoBehaviour {

	void Update ()
	{
		if (scr_main._f == null)
			SceneManager.LoadScene (SceneManager.GetActiveScene ().name);
		else if (scr_main._f.hasLevelLoaded) {
			if (scr_main._f.nextSpawn == -1)
				scr_main._f.nextSpawn = 0;
			scr_main.DPrint ("SP:" + scr_main._f.nextSpawn);
			Transform spawnPos = transform.GetChild (scr_main._f.nextSpawn).transform;
			scr_summon.f_summon.s_player (spawnPos.position, spawnPos.eulerAngles);
			scr_main._f.nextSpawn = -1;
			this.enabled = false;
		}
	}
}