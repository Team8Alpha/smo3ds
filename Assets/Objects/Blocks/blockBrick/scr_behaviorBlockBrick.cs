using UnityEngine;

public class scr_behaviorBlockBrick : MonoBehaviour {

	Animator anim;
	int timerFrame = 0;
	int hitCount = 0;
	bool isActive = true;

	public int FrameLimit = 10; //10 * 30 = 300

	void Start() {
		anim = GetComponent<Animator> ();
		FrameLimit *= 30;
		this.enabled = false;
	}

	void DoKill(){
		GameObject.Instantiate(Resources.Load<GameObject>("Objects/objBlockBrickBreak"), transform.position, transform.rotation);
		Destroy(gameObject);
	}

	void DoIsEmpty(){
		scr_summon.s.s_object(9, transform.position, transform.eulerAngles);
		anim.Play("brickUp");
		Destroy(gameObject);
	}

	void SpawnCoins(int numCoins) {
		Vector3 coinSpawnPos = new Vector3(transform.position.x, transform.position.y + 0.8f, transform.position.z);
		var coin = scr_summon.s.s_object(0, coinSpawnPos, transform.eulerAngles).GetComponent<scr_behaviorCoin>();
		coin.currentState = 1;
	}

	void Update() {
		timerFrame++;
		if (timerFrame >= FrameLimit) {
			isActive = false;
			this.enabled = false;
		}
	}

	public void OnTouch(int type) {
		if (type == 1 || type == 4) {
			if (FrameLimit == 0) {
				DoKill ();
			} else {
				if (hitCount == 0)
					this.enabled = true;
				anim.Play ("brickUp");
				SpawnCoins (1);
				hitCount++;
				if (!isActive)
					DoIsEmpty ();
			}
		}
	}
}
