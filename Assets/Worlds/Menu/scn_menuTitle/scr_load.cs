using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class scr_load : MonoBehaviour
{
	public scr_title titleMario;
	AudioSource marioAudio;
	public GameObject spr_rotMap;
	public GameObject spr_shade;
	Material mat_rotMap;
	Material mat_shade;
	bool bvar0 = false;

	void Start(){
		marioAudio = titleMario.GetComponent<AudioSource> ();
		mat_rotMap = spr_rotMap.gameObject.GetComponent<MeshRenderer> ().material;
		mat_shade = spr_shade.gameObject.GetComponent<MeshRenderer> ().material;
		scr_gameInit.globalValues.focusOff ();
	}
	void LateUpdate(){
		if(!bvar0) if (!marioAudio.isPlaying&&scr_fadefull._f.isDone) {
			scr_gameInit.globalValues.focusOn ();
			for(int i = 0; i!=4; i++) transform.GetChild (i).gameObject.SetActive (true);
			bvar0 = true;
		}
	}

	public float speed;
	public Color startColor, endColor;
	public Color startColorShade, endColorShade;
	float startTime;

	private IEnumerator ChangeEngineColour()
	{
		float tick = 0f;
		while (mat_rotMap.color != endColor)
		{
			tick += Time.unscaledDeltaTime * speed;
			mat_rotMap.color = Color.Lerp(startColor, endColor, tick);
			mat_shade.color = Color.Lerp(startColorShade, endColorShade, tick);
			yield return null;
		}
	}

    public void LoadResume(string sceneName)
    {
		StartCoroutine (ChangeEngineColour());
		scr_loadScene._f.nextScene = "scn_capMain0";//button?
		scr_gameInit.globalValues.focusOff();
		scr_gameInit.globalValues.dbg_enemyCount=0;
		SceneManager.LoadScene ("scn_loadShip", LoadSceneMode.Additive);
		titleMario.anim.Play("titleEnd");
    }
}