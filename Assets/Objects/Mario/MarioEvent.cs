using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum eEventPl
{
    wait, // wait and no input
    walk, // all the controlling
    demoMoon, // demo for moon get normal
    die, // death 
    hack, // hack
    unhack, // unhack
}

public class MarioEvent : MonoBehaviour
{
    eEventPl myEvent = eEventPl.wait;
    eEventPl myPrevEvent = eEventPl.wait;
    byte mySubEvent = 0;
    public static MarioEvent s;
    MarioController mario;
    scrBehaviorCappy cappy;

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
            case eEventPl.walk:
                break;
            case eEventPl.hack:
                if (mario.key_backR) SetEvent(eEventPl.unhack);
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
            case eEventPl.walk:
                MarioController.s.enabled = true;
                break;
            case eEventPl.hack:
                switch (mySubEvent)
                {
                    case 0:
                        mario.SetState(eStatePl.CaptureFly);
                        break;
                    case 1:
                        mario.SetState(eStatePl.Ground);
                        cappy.hackedObj.SendMessage("SetState", 6);
                        mario.SetVisible(false);
                        break;
                }
                break;
            case eEventPl.unhack:
                cappy.hackedObj.SendMessage("SetState", 7);
                cappy.SetState(eStateCap.Return);
                transform.Translate(0, 0, -2); //TODO: jump out of hack obj
                mario.ResetSpeed();
                mario.SetState(eStatePl.Ground);
                mario.SetVisible(true);
                mario.SetCap(false);
                mario.isBlocked = false;
                mario.isBlockBlocked = false;
                mario.isHacking = false;
                break;
        }
    }
}