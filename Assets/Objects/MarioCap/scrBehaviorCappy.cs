using System;
using UnityEngine;

public enum eStateCap
{
    Wait,                           // no movement, invisible.
    Throw,                          // flying forward
    FlyWait,                        // flying static.
    Return,                         // fly back to mario
    Hack,                           // captured, original calls it internally "hacking"
    HackAfter,                      // uncapturing
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
    private Transform myParent;     // saves parent, for mario switching.
    public GameObject hackedObj;    // object posessed by cappy
    Rigidbody rb;
    CharacterController charc;

    //private const float numOffsetY = 0.6f; // y offset at marios

    private Vector3 posOrigin;      // start position before flying
    private float numTimer = 0;     // timer for states
    //private string strAnimLast = "";  // last animation set
    public Transform mountpoint;       // mount point for possession/hacking

    public bool isHacking = false;


    void Awake()
    {
        SetVisible(false);

        mAnim = GetComponent<Animator>();
        mAudio = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody>();
        charc = GetComponent<CharacterController>();
        charc.detectCollisions = false;

        objBone = transform.GetChild(0);
        myParent = transform.parent;
        mario = MarioController.marioObject;
        tMario = mario.transform;
        mario.cappy = this;

        SetState(eStateCap.Wait);
        SetParent(mario.transform);
    }

    void Update()
    {
        if (scr_main._f.isFocused)
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
                            if (Vector3.Distance(posOrigin, transform.position) < 4 && charc.velocity.magnitude > 0f)
                                OnMove(0, 0, 2);
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
                SetParent();
                SetVisible(false);
                SetCollision(false);
                SetAnim("default");
                break;
            case eStateCap.Throw:
                switch (mySubState)
                {
                    case 0:
                        string nameAnim = "spinCapStart";
                        if (mario.jumpType > 0) nameAnim = "spinCapJumpStart";
                        SetAnim(nameAnim);
                        mario.SetAnim(nameAnim, 0.03f, 0.3f);
                        mario.SetCap(false);
                        mario.SetHand(1, 0, false);
                        mario.SetHand(1, 1, true);
                        mario.isFreezeFall = true;
                        SetParent(mario.transform); // follow mario
                        break;
                    case 1:
                        SetAnim("default");
                        SetRotate(true);
                        SetParent(); // reset parent 
                        mario.isFreezeFall = false;
                        mario.SetHand(1, 1, false);
                        mario.SetHand(1, 0, true);
                        transform.position = mario.transform.position;
                        transform.rotation = tMario.rotation;
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
                        SetParent(null, false);
                        SetAnim("default", 0.2f);
                        SetRotate(true);
                        break;
                    case 1:
                        SetParent(tMario);
                        SetAnim("CatchCap");
                        mario.SetAnim("CatchCap", 0.05f, 1);
                        mario.SetCap(false);
                        SetRotate(false);
                        break;
                }
                break;
            case eStateCap.Hack:
                SetRotate(false);
                SetAnim("capture");
                scr_manAudio._f.PlaySelfSND(ref mAudio, eSnd.CappyHacked, false, true);
                break;
            case eStateCap.HackAfter:
                scr_main._f.capMountPoint = "";
                var Mustache = hackedObj.transform.GetChild(1).GetChild(0);
                if (Mustache.name == "Mustache" || Mustache.name == "Mustache__HairMT")
                    Mustache.gameObject.SetActive(false); //if mustache, place it at index 0
                isHacking = false;
                SetCollision(false);
                SetState(eStateCap.Return);
                break;
        }
    }
    public void SetParent(Transform parent = null, bool resetPos = true)
    {
        if (parent != null) transform.SetParent(parent);
        else transform.SetParent(myParent);
        if (resetPos)
        {
            transform.localPosition = Vector3.zero;
            transform.localEulerAngles = Vector3.zero;
            transform.position = transform.parent.position;
            transform.rotation = transform.parent.rotation;
            transform.localScale = Vector3.one;

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
                scr_manAudio._f.PlaySelfSND(ref mAudio, eSnd.CappySpin, true);
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
        foreach(Collider obj in GetComponents<Collider>())
        {
            obj.enabled = boolean;
        }
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
        mountpoint = collis.transform.Find(scr_main._f.capMountPoint);
        if (mountpoint == null)
        {
            scr_main.DPrint("Cap: No mount at " + collis.gameObject.name + "/" + scr_main._f.capMountPoint);
            return;
        }

        hackedObj = collis.gameObject;
        if (hackedObj.GetComponent<Collider>() != null)
            hackedObj.GetComponent<Collider>().enabled = false;
        if (hackedObj.GetComponent<Rigidbody>() != null)
            hackedObj.GetComponent<Rigidbody>().useGravity = false;
        SetParent(mountpoint);
        SetCollision(false);

        hackedObj.SendMessage("OnCapHacked"); //send OnCapHacked event to object
        if (objParam.isHack) MarioController.marioObject.isHacking = true; //TODO: hacking event.
        else mAnim.Play("hookStart");

        GameObject Mustache = hackedObj.transform.GetChild(1).GetChild(0).gameObject;
        if (Mustache.name == "Mustache" || Mustache.name == "Mustache__HairMT") Mustache.SetActive(true); //if mustache, place it at index 0

        SetState(eStateCap.Hack);

        scr_main.DPrint("cap: mount at " + collis.gameObject.name + "/" + scr_main._f.capMountPoint);
        isHacking = true;
    }
}