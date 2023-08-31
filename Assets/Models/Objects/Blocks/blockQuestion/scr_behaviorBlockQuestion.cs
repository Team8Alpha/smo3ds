using UnityEngine;

public class scr_behaviorBlockQuestion : MonoBehaviour {

	Animator anim;
	int timerFrame = 0;
	int hitCount = 0;
	bool isActive = true;

	public int FrameLimit = 10; //10 * 30 = 300
	const int CoinInterval = 10;
	private int CoinInvFrame = 0;

	void Start() {
		anim = GetComponent<Animator> ();
		FrameLimit *= 30;
	}

	void DoIsEmpty(){
		scr_summon.f_summon.s_object(9, transform.position, transform.eulerAngles);
		Destroy(gameObject);
	}

	void SpawnCoins(int numCoins) {
		Vector3 coinSpawnPos = new Vector3(transform.position.x, transform.position.y + 0.8f, transform.position.z);
		var coin = scr_summon.f_summon.s_object(0, coinSpawnPos, transform.eulerAngles).GetComponent<scr_behaviorCoin>();
		coin.currentState = 1;
	}

	void Update() {
		timerFrame++;
		if (timerFrame >= FrameLimit) {
			isActive = false;
			this.enabled = false;
		}
	}

	public void OnTouch() {
		if (hitCount == 0) this.enabled = true;
		if (timerFrame - CoinInvFrame >= CoinInterval || hitCount == 0 || !isActive) {
			CoinInvFrame = timerFrame;
			anim.Play("up");
			SpawnCoins(1);
			hitCount++;
			if (!isActive)
				DoIsEmpty();
		}
	}
}
