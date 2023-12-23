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
    [HideInInspector] public eEventPl myEvent = eEventPl.wait;
    [HideInInspector] public eEventPl myPrevEvent = eEventPl.wait;
    [HideInInspector] public byte mySubEvent = 0;
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
        myEvent = eventID;
        mySubEvent = subEventID;
        switch(myEvent)
        {
            case eEventPl.wait:
                MarioController.s.enabled = false;
                break;
            case eEventPl.control:
                MarioController.s.enabled = true;
                mario.SetState(eStatePl.Ground);
                mario.isInputBlocked = false;
                break;
            case eEventPl.hack:
                switch (mySubEvent)
                {
                    case 0:
                        mario.SetAnim("captureFly");
                        MarioController.s.enabled = false;
                        fVar0 = Time.time;
                        mario.isHacking = true;
                        break;
                    case 1:
                        mario.SetState(eStatePl.Ground);
                        cappy.hackedObj.SendMessage("SetState", 6);
                        mario.SetVisible(false);
                        MarioController.s.enabled = true;
                        break;
                }
                break;
            case eEventPl.unhack:
                cappy.hackedObj.SendMessage("SetState", 7);
                cappy.SetState(eStateCap.Return);
                cappy.isHacking = false;
                transform.Translate(0, 0, -2); //TODO: jump out of hack obj
                mario.ResetSpeed();
                mario.SetVisible(true);
                mario.SetCap(false);
                GameObject mustache = cappy.hackedObj.transform.GetChild(1).GetChild(0).gameObject;
                if (mustache.name == "Mustache" || mustache.name == "Mustache__HairMT") mustache.SetActive(false);
                SetEvent(eEventPl.control);
                break;
        }
    }
    // Bezier function
    Vector3 Bezier(Vector3 a, Vector3 b, Vector3 c, float t)
    {
        float s = 1.0f - t;
        return s * s * a + 2.0f * s * t * b + t * t * c;
    }
}