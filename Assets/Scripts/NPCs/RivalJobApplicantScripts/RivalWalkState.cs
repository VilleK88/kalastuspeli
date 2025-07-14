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
        if(rival.currentMarkerObject == null || rival.fishing)
        {
            //ToRivalIdleState();
            ToRivalFishingState();
            return;
        }

        if (!rival.agent.hasPath)
        {
            if(rival.agent.enabled && rival.agent.isOnNavMesh)
                rival.agent.SetDestination(rival.currentMarkerObject.transform.position);
        }
        /*if (rival.agent.enabled && rival.agent.isOnNavMesh)
            rival.agent.SetDestination(rival.currentMarkerObject.transform.position);*/
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

    public void ToRivalFishingState()
    {
        Debug.Log("To fishing state");
        rival.agent.ResetPath();
        rival.fishingState.idleStartTime = Time.time;
        rival.anim.SetBool("Walk", false);
        rival.anim.SetTrigger("FishingCast");
        rival.anim.SetBool("FishingIdle", true);
        rival.currentState = rival.fishingState;
    }

    public IEnumerator DelayedStateChange(float time)
    {
        rival.anim.SetBool("Walk", false);
        yield return new WaitForSeconds(time);
        rival.anim.SetTrigger("FishingCast");
        rival.anim.SetBool("FishingIdle", true);
        Debug.Log("To rival fishing state");
    }
}