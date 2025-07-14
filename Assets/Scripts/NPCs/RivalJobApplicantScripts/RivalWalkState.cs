using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RivalWalkState : IRivalState
{
    RivalJobApplicant rival;

    public RivalWalkState(RivalJobApplicant thisRival)
    {
        this.rival = thisRival;
    }

    public void UpdateState()
    {
        if(rival.currentMarkerObject == null)
        {
            ToRivalIdleState();
            return;
        }

        if (!rival.agent.hasPath)
        {
            if(rival.agent.enabled && rival.agent.isOnNavMesh)
                rival.agent.SetDestination(rival.currentMarkerObject.transform.position);
        }
    }

    public void ToRivalIdleState()
    {
        rival.agent.ResetPath();
        rival.idleState.idleStartTime = Time.time;
        rival.anim.SetBool("Walk", false);
        rival.currentState = rival.idleState;
    }

    public void ToRivalWalkState()
    {

    }
}