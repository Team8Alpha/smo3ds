﻿using System;
using UnityEngine;

public enum capState
{
    Wait,                           // no movement, invisible.
    Throw,                          // flying forward
    FlyWait,                        // flying static.
    Return,                         // fly back to mario
    Hack,                           // captured, original calls it internally "hacking"
    Jump                            // cap jump
}
public class scrBehaviorCappy : MonoBehaviour
{
    public Animator mAnim;         // anim component
    private Transform tMario;       // player reference
    private MarioController mario;  // mario reference
    private AudioSource mAudio;     // audio component
    private Transform objBone = null;      // bone that is assigned to cap when hacked.
    public static capState myState; // cap state
    [HideInInspector]
    public int mySubState;          // cap states state
    private Transform myParent;     // saves parent, for mario switching.
    public GameObject hackedObj;    // object posessed by cappy
    Rigidbody rb;

    //private const float numOffsetY = 0.6f; // y offset at marios

    private Vector3 posOrigin;      // start position before flying
    private float numTimer = 0;     // timer for states
    //private string strAnimLast = "";  // last animation set
    public string mountpoint;       // mount point for possession/hacking

    public bool isHacking = false;


    void Awake()
    {
        mAnim = GetComponent<Animator>();
        mAudio = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody>();

        objBone = transform.GetChild(0);
        myParent = transform.parent;
        mario = MarioController.marioObject;
        tMario = mario.transform;
        mario.cappy = this;

        transform.localScale = Vector3.one;

        SetState(capState.Wait);
        SetParent(mario.transform);
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
                            float varAnimTime = GetAnimTime();
                            mario.rb.AddForce(0.6f * Vector3.up, ForceMode.Impulse);
                            if (varAnimTime > 0.6f && varAnimTime < 1) SetState(capState.Throw, 1);
                            break;
                        case 1:
                            if (Vector3.Distance(posOrigin, transform.position) < 4)
                                OnMove(0, 0, 2);
                            else SetState(capState.FlyWait);
                            break;
                    }
                    break;
                case capState.FlyWait:
                    if (numTimer < Application.targetFrameRate / 2) numTimer++;
                    else if (!mario.key_cap) { numTimer = 0; SetState(capState.Return); }
                    break;
                case capState.Return:
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

    void OnMove(float x, float y, float z)
    {
        transform.Translate(x, y, z);
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
                SetAnim("default");
                break;
            case capState.Throw:
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
                        SetRotate(false);
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
        if (resetPos)
        {
            transform.localPosition = Vector3.zero;
            transform.position = parent == null ? mario.transform.position : parent.position;
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
    void SetCollision(bool boolean)
    {
        gameObject.GetComponents<Collider>()[1].enabled = boolean;
        gameObject.GetComponents<Collider>()[0].enabled = boolean;
    }
    float GetAnimTime()
    {
        return mAnim.GetCurrentAnimatorStateInfo(1).normalizedTime;
    }

    void OnTriggerEnter(Collider collis)
    {
        paramObj objParam;
        if ((objParam = collis.gameObject.GetComponent<paramObj>()) == null) return;
             mountpoint = "";
        if (objParam.isTouch) collis.gameObject.SendMessage("OnTouch", 1);
        if (!objParam.isCapTrigger || isHacking) return;
        if (mountpoint == "" || (collis.transform.Find(mountpoint) == null))
        {
            scr_main.DPrint("Cap: No mount at " + collis.gameObject.name + "/" + mountpoint);
            return;
        }

        hackedObj = collis.gameObject;
        if (hackedObj.GetComponent<Collider>() != null)
            hackedObj.GetComponent<Collider>().enabled = false;
        if (hackedObj.GetComponent<Rigidbody>() != null)
            hackedObj.GetComponent<Rigidbody>().useGravity = false;
        SetCollision(false);

        hackedObj.SendMessage("OnCapHacked"); //send OnCapHacked event to object
        if (objParam.isHack) MarioController.marioObject.isHacking = true; //TODO: hacking event.
        else mAnim.Play("hookStart");
        SetState(capState.Hack);

        GameObject Mustache = hackedObj.transform.GetChild(1).GetChild(0).gameObject;
        if (Mustache.name == "Mustache" || Mustache.name == "Mustache__HairMT") Mustache.SetActive(true); //if mustache, place it at index 0
        scr_main.DPrint("CAPMOUNT AT " + collis.gameObject.name + "/" + mountpoint);
        isHacking = true;
    }
}