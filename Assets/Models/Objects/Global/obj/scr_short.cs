using UnityEngine;
using System.Collections;

public class scr_short : MonoBehaviour
{
	void Start(){
		this.enabled = false;
	}
	void DoKill(){
		Destroy (gameObject);
	}
	void DoParKill(){
		Destroy (transform.parent.gameObject);
	}
	void CoinUp(){
		scr_main._f.coinsCount++;
	}
}

