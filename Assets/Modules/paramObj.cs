using UnityEngine;
using System;

public class paramObj : MonoBehaviour {

	[SerializeField]
	private float posCenterV = 0;
	public bool isTouch = false;
	public bool isCapture = false;
	public bool isHack = true;
	public bool isLOD = true;

	public float GetPosCenterV()
    {
		return posCenterV + transform.position.y;
    }
	void Awake(){
		try
		{
			if (isLOD) transform.GetChild(1).gameObject.SetActive(false);
		} catch(Exception e)
        {
			Debug.Log("LOD ERROR: "+gameObject.name+ " HAS INVALID STRUCTURE: "+e);
        }
		this.enabled = false;
	}
}
