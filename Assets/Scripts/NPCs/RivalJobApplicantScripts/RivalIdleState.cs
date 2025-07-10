using UnityEngine;

public class RivalIdleState : IRivalState
{
    RivalJobApplicant rival;
    public float idleStartTime;
    float maxIdleTime = 2;

    public RivalIdleState(RivalJobApplicant thisRival)
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
        Debug.Log("back to walk state");
        rival.agent.SetDestination(rival.currentMarkerObject.transform.position);
        rival.anim.SetBool("Walk", true);
        rival.currentState = rival.walkState;
    }
}