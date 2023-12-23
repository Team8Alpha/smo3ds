using UnityEngine;

public class scr_globalLoad : MonoBehaviour {

	// Use this for initialization
	[RuntimeInitializeOnLoadMethod]
	void Start () {
		GameObject sum_globinit = Resources.Load<GameObject> ("objGlobal");
		if (FindObjectOfType<scr_main>() == null) { 
			GameObject.Instantiate (sum_globinit); 
		}
		scr_manageData.s.LoadLevel();
		Destroy(gameObject);
	}
}