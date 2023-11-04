using System;
using UnityEngine;

public enum capState
{
    Wait,                           // no movement, invisible.
    Transition,                     // transitions between actions like throwing
    Throw,                          // flying forward
    FlyWait,                        // flying static.
    Return,                         // fly back to mario
    Jump,                           // cap jump
    Hack                            // captured, original calls it internally "hacking"
}
public class scrBehaviorCappy : MonoBehaviour
{
    private Animator mAnim;         // anim component
    private Transform mario;        // player reference
    private AudioSource mAudio;     // audio component
    private Transform objBone;      // bone that is assigned to cap when hacked.
    public static capState myState; // cap state
    public int mySubState;          // cap states state

    void Awake()
    {
        //MarioController.marioObject.cappy = this;
        mAnim = GetComponent<Animator>();
        mAudio = GetComponent<AudioSource>();
        objBone = transform.GetChild(0);
    }

    void Update()
    {
        if (scr_main._f.isFocused)
        {
            switch (myState)
            {
                case capState.Wait:
                    break;
                case capState.Transition:
                    break;
                case capState.Throw:
                    break;
                case capState.FlyWait:
                    break;
                case capState.Return:
                    break;
                case capState.Hack:
                    break;
            }
        }
    }

    public void SetState(capState state, int subState)
    {
        myState = state;
        switch (myState)
        {
            case capState.Wait:
                break;
            case capState.Transition:
                break;
            case capState.Throw:
                break;
            case capState.FlyWait:
                break;
            case capState.Return:
                break;
            case capState.Hack:
                break;
        }
    }
}
