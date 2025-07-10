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
        if(rival.currentMarkerObject == null)
        {
            ToRivalIdleState();
            return;
        }

        if (!rival.agent.hasPath)
        {
            rival.agent.SetDestination(rival.currentMarkerObject.transform.position);
            Debug.Log("Find new path in Walk State");
        }

        if(Vector3.Distance(rival.transform.position, rival.lastPosition) < 0.5f)
        {
            rival.stuckTimer += Time.deltaTime;
            if(rival.stuckTimer > 3)
            {
                rival.JumpToTarget(rival.currentMarkerObject.transform.position);
                rival.stuckTimer = 0;
            }
        }
        else
        {
            rival.stuckTimer = 0;
            rival.lastPosition = rival.transform.position;
        }
    }

    public void ToRivalIdleState()
    {
        rival.agent.ResetPath();
        Debug.Log("Reset path and go to Idle State");
        rival.idleState.idleStartTime = Time.time;
        rival.anim.SetBool("Walk", false);
        rival.currentState = rival.idleState;
    }

    public void ToRivalWalkState()
    {

    }
}