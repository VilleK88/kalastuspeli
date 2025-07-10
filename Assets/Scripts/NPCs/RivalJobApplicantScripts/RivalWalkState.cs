using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RivalWalkState : IRivalState
{
    RivalJobApplicant rival;

    public RivalWalkState(RivalJobApplicant thisRival)
    {
        this.rival = thisRival;
    }

    public void UpdateState()
    {

    }

    public void ToRivalIdleState()
    {
        rival.idleState.idleStartTime = Time.time;
        rival.anim.SetBool("Walk", false);
        rival.currentState = rival.idleState;
    }

    public void ToRivalWalkState()
    {

    }
}