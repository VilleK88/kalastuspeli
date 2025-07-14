using UnityEngine;

public class RivalFishingState : IRivalState
{
    RivalJobApplicant rival;
    public float idleStartTime;
    float maxIdleTime = 12;

    public RivalFishingState(RivalJobApplicant thisRival)
    {
        this.rival = thisRival;
    }

    public void UpdateState()
    {
        if (Time.time - idleStartTime >= maxIdleTime)
        {
            ToRivalWalkState();
        }
    }

    public void ToRivalIdleState()
    {

    }

    public void ToRivalWalkState()
    {
        rival.StartCoroutine(rival.DestroyMarkerAndTransition());

        /*rival.anim.SetBool("FishingIdle", false);
        rival.FindClosestMarker();

        if (rival.currentMarkerObject != null)
        {
            Debug.Log("back to walk state");
            rival.agent.SetDestination(rival.currentMarkerObject.transform.position);
            rival.anim.SetBool("Walk", true);
            rival.currentState = rival.walkState;
        }
        else
        {
            idleStartTime = Time.time;
        }*/
    }

    public void ToRivalFishingState()
    {

    }
}