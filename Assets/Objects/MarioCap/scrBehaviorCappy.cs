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
    public Animator mAnim;         // anim component
    private Transform tMario;       // player reference
    private MarioController mario;  // mario reference
    private AudioSource mAudio;     // audio component
    private Transform objBone;      // bone that is assigned to cap when hacked.
    public static capState myState; // cap state
    [HideInInspector]
    public int mySubState;          // cap states state
    private Transform myParent;     // saves parent, for mario switching.
    [SerializeField]
    private Transform objMarioOrigin; // origin at marios

    //private const float numOffsetY = 0.6f; // y offset at marios

    private Vector3 posOrigin;      // start position before flying
    private float numTimer = 0;     // timer for states
    private string strAnimLast = "";// last animation set


    void Awake()
    {
        mAnim = GetComponent<Animator>();
        mAudio = GetComponent<AudioSource>();
        objBone = transform.GetChild(0);
        myParent = transform.parent;
        mario = MarioController.marioObject;
        tMario = mario.transform;
        mario.cappy = this;
        transform.localScale = Vector3.one;
        SetState(capState.Wait);
        SetVisible(false);
    }

    void Update()
    {
        if (scr_main._f.isFocused)
        {
            switch (myState)
            {
                case capState.Wait: // 5.5, 1
                    if (mario.key_cap) SetState(capState.Throw);
                    break;
                case capState.Throw:
                    switch (mySubState)
                    {
                        case 0:
                            float varAnimTime = mAnim.GetCurrentAnimatorStateInfo(1).normalizedTime;
                            if (varAnimTime > 0.6f && varAnimTime < 1) SetState(capState.Throw, 1);
                            break;
                        case 1:
                            if (Vector3.Distance(posOrigin, transform.position) < 5f)
                                transform.Translate(0, 0, 1f);
                            else if (!mario.key_cap) SetState(capState.FlyWait);
                            break;
                    }
                    break;
                case capState.FlyWait:
                    if (numTimer < Application.targetFrameRate) numTimer++;
                    else { numTimer = 0; SetState(capState.Return); }
                    break;
                case capState.Return:
                    switch (mySubState)
                    {
                        case 0:
                            transform.position = Vector3.MoveTowards(transform.position, objMarioOrigin.position, 1);
                            if (Vector3.Distance(transform.position, objMarioOrigin.position) < 0.1f)
                            {
                                SetState(myState, 1);
                            }
                            break;
                        case 1:
                            if (!CheckIsAnim())
                            {
                                mario.SetCap(true);
                                SetState(capState.Wait);
                            }
                            break;
                    }
                    break;
                case capState.Hack:
                    break;
            }
        }
    }

    public void SetState(capState state, int subState = 0)
    {
        myState = state;
        mySubState = subState;
        SetVisible(true);
        switch (myState)
        {
            case capState.Wait:
                SetRotate(false);
                SetParent();
                SetVisible(false);
                transform.localPosition = Vector3.zero;
                break;
            case capState.Throw:
                switch (mySubState)
                {
                    case 0:
                        SetAnim("spinCapStart");
                        mario.SetAnim("spinCapStart", 0.03f, 0.5f);
                        mario.SetCap(false);
                        SetParent(mario.transform); // follow mario
                        break;
                    case 1:
                        SetAnim("default");
                        SetRotate(true);
                        SetParent(); // reset parent 
                        transform.position = objMarioOrigin.position;
                        transform.rotation = tMario.rotation;
                        posOrigin = transform.position;
                        break;
                }
                break;
            case capState.FlyWait:
                SetAnim("stay", 0.2f);
                break;
            case capState.Return:
                switch (mySubState)
                {
                    case 0:
                        SetAnim("default", 0.2f);
                        SetRotate(true);
                        break;
                    case 1:
                        SetParent(tMario);
                        SetAnim("CatchCap");
                        mario.SetAnim("CatchCap", 0.05f, 1);
                        mario.SetCap(false);
                        break;
                }
                break;
            case capState.Hack:
                break;
        }
    }
    public void SetParent(Transform parent = null, bool resetPos = true)
    {
        if (parent != null) transform.SetParent(parent);
        else transform.SetParent(myParent);
        if (resetPos) transform.localPosition = Vector3.zero;
    }
    void SetAnim(string animName, float transitionTime = 0.1f, float animSpeed = 1)
    {
        if (transitionTime == 0) mAnim.Play(animName, 1);
        else mAnim.CrossFade(animName, transitionTime, 1);
        mAnim.speed = animSpeed;
        strAnimLast = animName;
    }
    void SetRotate(bool boolean)
    {
        if(!mAnim.GetCurrentAnimatorStateInfo(0).IsName("rotate")) mAnim.Play("rotate", 0);
    }
    void SetVisible(bool boolean)
    {
        transform.GetChild(0).gameObject.SetActive(boolean);
        transform.GetChild(1).gameObject.SetActive(boolean);
        gameObject.GetComponent<Animator>().enabled = boolean;
    }
    bool CheckIsAnim()
    {
        return (mAnim.GetCurrentAnimatorStateInfo(1).normalizedTime < 1) ? true : false;
    }

}