using UnityEngine;

public class scr_behaviorCheckpoint : MonoBehaviour {

	public int numSpawnPoint = 0; //which spawn point is this checkpoint adressed to.
	private bool wasActivated = false;
	[SerializeField] private Material mat_after; 

	Animator anim;
	// Use this for initialization
	void Start() {
		anim = GetComponent<Animator>();
		anim.Play("before");
	}

	void OnTriggerEnter(Collider collis)
    {
		if (!wasActivated && collis.tag == "Player"){
			anim.Play("get");
			transform.GetChild(2).gameObject.GetComponent<Renderer>().material = mat_after;
			scr_main._f.lastCheckpoint = numSpawnPoint;
			scr_manageData._f.Save();
			wasActivated = true;
			//MarioController.marioObject.setAnim (" ");
		}
	}
}