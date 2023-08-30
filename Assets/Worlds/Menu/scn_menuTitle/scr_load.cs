using UnityEngine;
using UnityEngine.SceneManagement;

public class scr_load : MonoBehaviour
{
	public void LoadResume()
    {
		StartCoroutine (scr_title._f.ChangeEngineColour());
		scr_loadScene._f.nextScene = "scn_capMain0";//button?
		scr_gameInit.globalValues.focusOff();
		scr_gameInit.globalValues.dbg_enemyCount=0;
		SceneManager.LoadScene ("scn_loadShip", LoadSceneMode.Additive);
		scr_title._f.GetComponent<Animator>().Play ("titleEnd");
    }
}