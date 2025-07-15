using System.Collections;
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
        if(rival.currentMarkerObject == null || rival.fishing)
        {
            ToRivalFishingState();
            return;
        }

        if (!rival.agent.hasPath)
        {
            Debug.Log("Rival agent does not have a path");
            if(rival.agent.enabled && rival.agent.isOnNavMesh)
            {
                Vector3 pointNearMarker = rival.GetRandomPointNearMarker(rival.currentMarkerObject.transform.position, 5, 10);
                rival.agent.SetDestination(pointNearMarker);
            }
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