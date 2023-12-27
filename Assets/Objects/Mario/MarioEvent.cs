using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum eEventPl
{
    wait, // wait and no input
    control, // all the controlling
    demoMoon, // demo for moon get normal
    die, // death 
    hack, // hack
    unhack, // unhack
}

public class MarioEvent : MonoBehaviour
{
    public eEventPl myEvent = eEventPl.wait;
    public eEventPl myPrevEvent = eEventPl.wait;
    public byte myPrevSubEvent = 0;
    public byte mySubEvent = 0;
    [HideInInspector] public static MarioEvent s;
    MarioController mario;
    scrBehaviorCappy cappy;

    float fVar0 = 0;

    void Start()
    {
        s = this;
        mario = MarioController.s;
        cappy = mario.cappy;
    }

    void Update()
    {
        switch (myEvent)
        {
            case eEventPl.wait:
                break;
            case eEventPl.control:
                break;
            case eEventPl.hack:
                switch (mySubEvent)
                {
                    case 0:
                        float distanceCovered = (Time.time - fVar0) / 1;

                        Vector3 posHackObj = cappy.hackedObj.transform.position;
                        Vector3 targetPosition = Bezier(transform.position, posHackObj + Vector3.up * 2 + ((transform.position - posHackObj).normalized), posHackObj, distanceCovered);

                        transform.position = targetPosition;

                        if (Vector3.Distance(transform.position, posHackObj) < 1f)
                        {
                            transform.position = posHackObj;
                            MarioEvent.s.SetEvent(eEventPl.hack, 1);
                        }
                        break;
                    case 1:
                        if (mario.key_backL) SetEvent(eEventPl.unhack);
                        break;
                }
                break;
        }
    }

    public void SetEvent(eEventPl eventID, byte subEventID = 0)
    {
        myPrevEvent = myEvent;
        myPrevSubEvent = mySubEvent;
        myEvent = eventID;
        mySubEvent = subEventID;
        switch (myEvent)
        {
            case eEventPl.wait:
                mario.enabled = false;
                break;
            case eEventPl.control:
                mario.enabled = true;
                mario.SetState(eStatePl.Ground);
                mario.isInputBlocked = false;
                break;
            case eEventPl.hack:
                switch (mySubEvent)
                {
                    case 0:
                        fVar0 = Time.time;
                        mario.SetAnim("captureFly");
                        mario.isHacking = true;
                        mario.enabled = false;
                        break;
                    case 1:
                        cappy.hackedObj.SendMessage("SetState", 6);
                        mario.SetState(eStatePl.Ground);
                        mario.SetVisible(false);
                        mario.enabled = true;
                        cappy.hackedObj.GetComponent<Collider>().enabled = false;
                        break;
                }
                break;
            case eEventPl.unhack:
                cappy.hackedObj.SendMessage("SetState", 7);
                cappy.SetState(eStateCap.UnHack);
                cappy.hackedObj.GetComponent<Collider>().enabled = true;

                transform.Translate(0, 0, -2); //TODO: jump out of hack obj

                mario.ResetSpeed();
                mario.SetVisible(true);
                mario.SetCap(false);

                GameObject mustache = cappy.hackedObj.transform.GetChild(1).GetChild(0).gameObject;
                if (mustache.name == "Mustache" || mustache.name == "Mustache__HairMT") mustache.SetActive(false);

                SetEvent(eEventPl.control);
                break;

            case eEventPl.demoMoon:
                switch (mySubEvent)
                {
                    case 0: // start
                        SetDelayEvent(3, myEvent, 1);

                        OnDemo(true);
                        scr_main.s.SetFocus(false);

                        MarioCam.s.confSmoothTime = 0.2f;
                        MarioCam.s.confCamDistance -= 2;
                        MarioCam.s.confYOffset -= 0.8f;
                        //MarioCam.s.additionalRot.x -= MarioCam.s.transform.eulerAngles.x * 0.8f - 30;
                        MarioCam.s.confIsWallBlock = false;
                        MarioCam.s.SetTransPl(true);

                        mario.SetAnim("demoShineGet");
                        if (mario.isHacking)
                        {
                            mario.SetVisible(true);
                            cappy.transform.GetChild(1).gameObject.layer = 0;
                            cappy.objCappyEyes.SetActive(false);
                        }
                        mario.rb.velocity = new Vector3(0, 0, 0);
                        mario.posGround = mario.transform.position.y;
                        mario.transform.rotation = Quaternion.Euler(mario.transform.eulerAngles.x, MarioCam.s.transform.eulerAngles.y+180, mario.transform.eulerAngles.x);

                        SetDelayEvent(1.3f, myEvent, 3);

                        scr_manAudio.s.PlaySND(eSnd.JnMoonGet);
                        break;
                    case 1: // add moon
                        scr_main.s.moonsCount++;
                        scr_manAudio.s.PlaySND(eSnd.CoinCollect);
                        mySubEvent = 0;
                        break;
                    case 2: // destroy
                        OnDemo(false);
                        scr_main.s.SetFocus(true);
                        MarioCam.s.confSmoothTime = MarioCam.s.defSmoothTime;
                        MarioCam.s.confCamDistance = MarioCam.s.defCamDistance;
                        MarioCam.s.confYOffset = MarioCam.s.defYOffset;
                        MarioCam.s.additionalRot.x = 0;
                        MarioCam.s.confIsWallBlock = true;
                        MarioCam.s.SetTransPl(false);
                        mario.transform.Rotate(0, -180, 0);
                        mario.GetComponent<Rigidbody>().useGravity = true;
                        if (mario.isHacking)
                        {
                            mario.SetVisible(false);
                            cappy.transform.GetChild(1).gameObject.layer = 18;
                            cappy.objCappyEyes.SetActive(true);
                        }
                        SetDelayEvent(3, myEvent, 4);

                        GoToPrevEvent(true);

                        mario.SetState(eStatePl.Falling);
                        break;
                    case 3:
                        scr_main.s.transform.GetChild(1).transform.GetChild(1).gameObject.SetActive(true);
                        GoToPrevEvent(true);
                        break;
                    case 4:
                        if(mario.enabled) scr_main.s.transform.GetChild(1).transform.GetChild(1).gameObject.SetActive(false);
                        GoToPrevEvent(true);
                        break;
                }

                break;

        }
    }
    // Bezier function
    Vector3 Bezier(Vector3 a, Vector3 b, Vector3 c, float t)
    {
        float s = 1.0f - t;
        return s * s * a + 2.0f * s * t * b + t * t * c;
    }

    public void SetDelayEvent(float tSec, eEventPl eventID, byte subEventID) { StartCoroutine(delayEvent(tSec, eventID, subEventID)); }
    IEnumerator delayEvent(float tSec, eEventPl eventID, byte subEventID)
    {
        float timer = 0f;

        while (timer < tSec)
        {
            timer += Time.unscaledDeltaTime;
            yield return null;
        }
        Debug.Log("MEv Call Event "+eventID+"."+subEventID+" after "+tSec+"s");
        SetEvent(eventID, subEventID);
    }
    void OnDemo(bool state)
    {
        mario.enabled = !state;
        mario.rb.isKinematic = state;
    }
    void GoToPrevEvent(bool silent = true)
    {
        if (silent)
        {
            myEvent = myPrevEvent;
            mySubEvent = myPrevSubEvent;
        }
        else SetEvent(myPrevEvent, myPrevSubEvent);
    }
}