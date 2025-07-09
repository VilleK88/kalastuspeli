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
        ReachTheClosestMarker();
    }

    public void ToRivalIdleState()
    {
        rival.currentState = rival.idleState;
    }

    public void ToRivalWalkState()
    {

    }

    public void ReachTheClosestMarker()
    {
        if(rival.currentDistance < 5)
        {
            Debug.Log("Marker reached");
            rival.DestroyCurrentMarker();
            ToRivalIdleState();
        }
    }
}