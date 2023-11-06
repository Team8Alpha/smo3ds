using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum plEvent
{
	Empty, // empty head? not moving.
	Move, // mario controller event.
	Demo, // just died
}
public class MarioEvent : MonoBehaviour {

	public static plEvent myEvent;
	public int myEventState = 0;
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
