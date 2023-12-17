using UnityEngine;

public class scr_behaviorCheckpoint : MonoBehaviour {

	public int numSpawnPoint = 0; //which spawn point is this checkpoint adressed to.
	private bool wasActivated = false;
	[SerializeField] private Material mat_after; 

	Animator anim;
	// Use this for initialization
	void Start() {
		anim = GetComponent<Animator>();
	}

    void OnTouch(int type)
	{

		if (type >= 1)
		{
			anim.Play("get");
			transform.GetChild(1).GetChild(1).gameObject.GetComponent<Renderer>().material = mat_after;
			scr_main._f.lastCheckpoint = numSpawnPoint;
			scr_manageData._f.Save();
			wasActivated = true;

			if (type == 1)
			{

			} else
			{
				//MarioController.marioObject.setAnim (" ");
			}
		}
	}
}