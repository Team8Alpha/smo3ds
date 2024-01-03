using System;
using UnityEngine;

public enum eStateCap
{
    Wait,                           // no movement, invisible.
    Throw,                          // flying forward
    FlyWait,                        // flying static.
    Return,                         // fly back to mario
    Hack,                           // captured, original calls it internally "hacking"
    UnHack,                         // 
    Jump                            // cap jump
}
public class scrBehaviorCappy : MonoBehaviour
{
    public Animator mAnim;         // anim component
    private Transform tMario;       // player reference
    private MarioController mario;  // mario reference
    private AudioSource mAudio;     // audio component
    private Transform objBone = null;      // bone that is assigned to cap when hacked.
    public static eStateCap myState; // cap state
    [HideInInspector]
    public int mySubState;          // cap states state
    public GameObject hackedObj;    // object posessed by cappy
    CharacterController charc;
    public GameObject objCappyEyes;

    //private const float numOffsetY = 0.6f; // y offset at marios

    private Vector3 posOrigin;      // start position before flying
    private float numTimer = 0;     // timer for states
    //private string strAnimLast = "";  // last animation set
    public Transform mountpoint;       // mount point for possession/hacking

    public bool isHacking = false;


    void Awake()
    {

        mAnim = GetComponent<Animator>();
        mAudio = GetComponent<AudioSource>();
        charc = GetComponent<CharacterController>();


        objBone = transform.GetChild(0);
        mario = MarioController.s;
        tMario = mario.transform;
        mario.cappy = this;

        SetVisible(false);
        SetState(eStateCap.Wait);
    }

    void Update()
    {
        if (Time.timeScale > 0)
        {
            switch (myState)
            {
                case eStateCap.Wait: // 5.5, 1

                    if (mario.key_cap) SetState(eStateCap.Throw);

                    break;
                case eStateCap.Throw:
                    switch (mySubState)
                    {
                        case 0:
                            float varAnimTime = GetAnimTime();
                            mario.rb.AddForce(0.6f * Vector3.up, ForceMode.Impulse);
                            if (varAnimTime > 0.6f && varAnimTime < 1) SetState(eStateCap.Throw, 1);
                            break;
                        case 1:
                            if (Vector3.Distance(posOrigin, transform.position) < 5 && charc.velocity.magnitude > 0f)
                                OnMove(0, 0, 1);
                            else
                                SetState(eStateCap.FlyWait);
                            break;
                    }
                    break;
                case eStateCap.FlyWait:
                    if (numTimer < Application.targetFrameRate / 2) numTimer++;
                    else if (!mario.key_cap) { numTimer = 0; SetState(eStateCap.Return); }
                    break;
                case eStateCap.Return:
                    switch (mySubState)
                    {
                        case 0:
                            Vector3 posTargetM = mario.transform.position; posTargetM.y++;
                            transform.position = Vector3.MoveTowards(transform.position, posTargetM, 1);
                            if (Vector3.Distance(transform.position, posTargetM) < 0.1f)
                                SetState(myState, 1);
                            break;
                        case 1:
                            if (GetAnimTime() > 1)
                            {
                                mario.SetCap(true);
                                SetState(eStateCap.Wait);
                            }
                            break;
                    }
                    break;
                case eStateCap.Hack:
                    break;
            }
        }
    }

    void OnMove(float x, float y, float z)
    {
        charc.Move(transform.forward * new Vector3(x, y, z).magnitude);
    }

    public void SetState(eStateCap state, int subState = 0)
    {
        myState = state;
        mySubState = subState;
        SetVisible(true);
        switch (myState)
        {
            case eStateCap.Wait:
                SetRotate(false);
                SetVisible(false);
                SetCollision(false);
                SetAnim("default");
                break;
            case eStateCap.Throw:
                switch (mySubState)
                {
                    case 0:
                        string nameAnim = "spinCapStart";
                        if (mario.wasGrounded == false)
                        {
                            nameAnim = "spinCapJumpStart";
                            mario.isFlyFreeze = true;
                        }
                        SetAnim(nameAnim);
                        SetCollision(false);
                        mario.SetAnim(nameAnim, 0.03f, 0.3f);
                        mario.SetCap(false);
                        mario.SetHand(1, 0, false);
                        mario.SetHand(1, 1, true);
                        transform.position = tMario.position;
                        transform.rotation = tMario.rotation;
                        break;
                    case 1:
                        isHacking = false;
                        SetAnim("default");
                        SetRotate(true);
                        SetCollision(true);
                        mario.isFlyFreeze = false;
                        mario.SetHand(1, 1, false);
                        mario.SetHand(1, 0, true);
                        posOrigin = transform.position;
                        OnMove(0, 0.5f, 1);
                        break;
                }
                break;
            case eStateCap.FlyWait:
                SetAnim("stay", 0.2f);
                break;
            case eStateCap.Return:
                switch (mySubState)
                {
                    case 0:
                        SetAnim("default", 0.2f);
                        SetRotate(true);
                        break;
                    case 1:
                        SetAnim("CatchCap");
                        SetRotate(false);
                        mario.SetAnim("CatchCap", 0.05f, 1);
                        if (isHacking)
                        {
                            if (hackedObj.GetComponent<Collider>() != null)
                                foreach (Collider coll in hackedObj.GetComponents<Collider>())
                                    coll.enabled = true;
                            isHacking = false;
                        }
                        break;
                }
                break;
            case eStateCap.Hack:
                SetRotate(false);
                SetAnim("capture");
                scr_manAudio.s.PlaySelfSND(ref mAudio, eSnd.CappyHacked, false, true);
                break;
            case eStateCap.UnHack:
                SetCollision(false);
                scr_main.s.capMountPoint = "";
                SetParent(tMario.parent, false);
                SetState(eStateCap.Return);
                break;
        }
    }
    public void SetParent(Transform parent, bool resetPos)
    {
        Vector3 posPrev = parent.position;
        Quaternion rotPrev = parent.rotation;
        Vector3 sclPrev = parent.localScale;
        transform.SetParent(parent);
        if (resetPos)
        {
            transform.position = parent.position;
            transform.rotation = parent.rotation;
            transform.localScale = Vector3.one;

        } else
        {
            transform.position = posPrev;
            transform.rotation = rotPrev;
            transform.localScale = sclPrev;
        }
    }
    void SetAnim(string animName, float transitionTime = 0.1f, float animSpeed = 1)
    {
        if (transitionTime == 0) mAnim.Play(animName, 1);
        else mAnim.CrossFade(animName, transitionTime, 1);
        mAnim.speed = animSpeed;
        //strAnimLast = animName;
    }
    bool GetIsAnim(string name, int layer)
    {
        return mAnim.GetCurrentAnimatorStateInfo(layer).IsName(name);
    }
    void SetRotate(bool boolean)
    {
        if (boolean)
        {
            if (!GetIsAnim("spin", 0))
            {
                mAnim.Play("spin", 0);
                scr_manAudio.s.PlaySelfSND(ref mAudio, eSnd.CappySpin, true);
            }
        }
        else
        {
            mAnim.Play("wait", 0);
            mAudio.Stop();
        }
    }
    void SetVisible(bool boolean)
    {
        transform.GetChild(0).gameObject.SetActive(boolean);
        transform.GetChild(1).gameObject.SetActive(boolean);
        gameObject.GetComponent<Animator>().enabled = boolean;
        gameObject.GetComponent<AudioSource>().enabled = boolean;
        SetCollision(boolean);
    }
    public void SetTransformOffset(float scale, Vector3 pos, Vector3 rot)
    {
        transform.localScale = new Vector3(scale, scale, scale);
        if(pos!=null) transform.localPosition = pos;
        if(rot!=null) transform.localEulerAngles = rot;
    }
    public void SetCollision(bool boolean)
    {
        charc.detectCollisions = boolean;
    }
    float GetAnimTime()
    {
        return mAnim.GetCurrentAnimatorStateInfo(1).normalizedTime;
    }

    void OnTriggerEnter(Collider collis)
    {
        paramObj objParam;
        if ((objParam = collis.gameObject.GetComponent<paramObj>()) == null) return;
        if (objParam.isTouch) collis.gameObject.SendMessage("OnTouch", 1);
        if (!objParam.isCapTrigger || isHacking) return;
        else collis.gameObject.SendMessage("OnCapTrigger");
        mountpoint = collis.transform.Find(scr_main.s.capMountPoint);
        if (mountpoint == null)
        {
            scr_main.DPrint("Cap: No mount at " + collis.gameObject.name + "/" + scr_main.s.capMountPoint);
            return;
        }

        hackedObj = collis.gameObject;
        if (hackedObj.GetComponent<Collider>() != null)
            foreach (Collider coll in hackedObj.GetComponents<Collider>())
                coll.enabled = false;
        SetParent(mountpoint, true);
        SetCollision(false);

        hackedObj.SendMessage("OnCapHacked"); //send OnCapHacked call to object
        if (objParam.isHack) MarioEvent.s.SetEvent(eEventPl.hack);
        else mAnim.Play("hookStart");

        GameObject Mustache = hackedObj.transform.GetChild(1).GetChild(0).gameObject;
        if (Mustache.name == "Mustache" || Mustache.name == "Mustache__HairMT") Mustache.SetActive(true); //if mustache, place it at index 0

        SetState(eStateCap.Hack);

        scr_main.DPrint("cap: mount at " + collis.gameObject.name + "/" + scr_main.s.capMountPoint);
        isHacking = true;
    }
}