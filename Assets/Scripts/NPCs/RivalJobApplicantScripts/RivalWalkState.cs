using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
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
        rival.currentState = rival.idleState;
    }

    public void ToRivalWalkState()
    {

    }
}