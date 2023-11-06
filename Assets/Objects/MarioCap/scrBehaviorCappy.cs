using System;
using UnityEngine;

public enum capState
{
    Wait,                           // no movement, invisible.
    Throw,                          // flying forward
    FlyWait,                        // flying static.
    Return,                         // fly back to mario
    Jump,                           // cap jump
    Hack                            // captured, original calls it internally "hacking"
}
public class scrBehaviorCappy : MonoBehaviour
{
    private Animator mAnim;         // anim component
    private Transform tMario;       // player reference
    private AudioSource mAudio;     // audio component
    private Transform objBone;      // bone that is assigned to cap when hacked.
    public static capState myState; // cap state
    [HideInInspector]
    public int mySubState;          // cap states state

    //private const float numOffsetY = 0.6f; // y offset at marios

    private Vector3 posOrigin;      // start position before flying
    private float numTimer = 0;     // timer for states


    void Start()
    {
        tMario = MarioController.marioObject.transform;
        MarioController.marioObject.cappy = this;
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
                case capState.Wait: // 5.5, 1
                    break;
                case capState.Throw:
                    if (Vector3.Distance(posOrigin, transform.position) < 5f)
                        transform.Translate(0, 0, 0.8f);
                    else SetState(capState.FlyWait);
                    break;
                case capState.FlyWait:
                    if (numTimer < 100 * Time.deltaTime) numTimer++;
                    else SetState(capState.Return);
                    break;
                case capState.Return:
                    transform.position = Vector3.MoveTowards(transform.position, new Vector3(tMario.position.x, tMario.position.y, tMario.position.z + 1), 1);
                    break;
                case capState.Hack:
                    break;
            }
        }
    }

    public void SetState(capState state, int subState = 0)
    {
        gameObject.SetActive(true);
        myState = state;
        switch (myState)
        {
            case capState.Wait:
                break;
            case capState.Throw:
                transform.position = tMario.position + tMario.forward * 1;
                transform.rotation = tMario.rotation;
                posOrigin = transform.position;
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