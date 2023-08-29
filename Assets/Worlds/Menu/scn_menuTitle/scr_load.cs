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

	void Start(){
		marioAudio = titleMario.GetComponent<AudioSource> ();
		mat_rotMap = spr_rotMap.gameObject.GetComponent<MeshRenderer> ().material;
		mat_shade = spr_shade.gameObject.GetComponent<MeshRenderer> ().material;
	}
	void LateUpdate(){
		if (!marioAudio.isPlaying) {
			gameObject.GetComponent<Canvas> ().enabled = true;
			transform.GetChild (0).gameObject.SetActive (true);
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
		SceneManager.LoadScene ("scn_loadRes", LoadSceneMode.Additive);
		titleMario.anim.Play("titleEnd");
    }
}