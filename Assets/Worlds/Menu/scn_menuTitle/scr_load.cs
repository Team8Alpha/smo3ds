using UnityEngine;
using UnityEngine.SceneManagement;

public class scr_load : MonoBehaviour
{
	public scr_title titleMario;
	AudioSource marioAudio;
	void Start(){
		marioAudio = titleMario.GetComponent<AudioSource> ();
	}
	void Update(){
		if (!marioAudio.isPlaying)
			gameObject.GetComponent<Canvas> ().enabled = true;
	}
    public void LoadResume(string sceneName)
    {
		scr_loadScene._f.nextScene = "scn_capMain0";//button?
		scr_gameInit.globalValues.focusOff();
		scr_gameInit.globalValues.dbg_enemyCount=0;
		SceneManager.LoadScene ("scn_loadRes", LoadSceneMode.Additive);
		titleMario.anim.Play("titleEnd");
    }
}