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


    void Start()
    {
        mAnim = GetComponent<Animator>();
        mAudio = GetComponent<AudioSource>();
        objBone = transform.GetChild(0);
        myParent = transform.parent;
        tMario = MarioController.marioObject.transform;
        MarioController.marioObject.cappy = this;
        transform.localScale = Vector3.one;
        gameObject.SetActive(false);
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
                    switch (mySubState)
                    {
                        case 0:
                            float varAnimTime = mAnim.GetCurrentAnimatorStateInfo(0).normalizedTime;
                            if (varAnimTime > 0.6f && varAnimTime < 1) SetState(capState.Throw, 1);
                            break;
                        case 1:
                            if (Vector3.Distance(posOrigin, transform.position) < 5f)
                                transform.Translate(0, 0, 0.8f);
                            else SetState(capState.FlyWait);
                            break;
                    }
                    break;
                case capState.FlyWait:
                    if (numTimer < 1000 * Time.deltaTime) numTimer++;
                    else { numTimer = 0; SetState(capState.Return); }
                    break;
                case capState.Return:
                    transform.position = Vector3.MoveTowards(transform.position, objMarioOrigin.position, 1);
                    if (Vector3.Distance(transform.position, objMarioOrigin.position) < 0.1f) SetState(capState.Wait);
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
        mySubState = subState;
        switch (myState)
        {
            case capState.Wait:
                SetRotate(false);
                SetAnim("default");
                SetParent();
                transform.localPosition = Vector3.zero;
                gameObject.SetActive(false);
                break;
            case capState.Throw:
                switch (mySubState)
                {
                    case 0:
                        SetAnim("spinCapStart");
                        MarioController.marioObject.SetAnim("spinCapStart");
                        SetParent(MarioController.marioObject.transform); // follow mario
                        break;
                    case 1:
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
                SetAnim("default", 0.2f);
                SetRotate(true);
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
    void SetAnim(string animName, float transitionTime = 0, float animSpeed = 1)
    {
        if (transitionTime == 0) mAnim.Play(animName, 0);
        else mAnim.CrossFade(animName, transitionTime, 0);
        mAnim.speed = animSpeed;
        strAnimLast = animName;
    }
    void SetRotate(bool boolean)
    {
        mAnim.Play("rotate", 1);
    }

}