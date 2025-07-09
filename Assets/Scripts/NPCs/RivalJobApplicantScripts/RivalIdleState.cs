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
        if(rival.currentMarkerObject == null)
        {
            ToRivalWalkState();
        }
    }

    public void ToRivalIdleState()
    {

    }

    public void ToRivalWalkState()
    {
        rival.FindClosestMarker();
        rival.currentState = rival.walkState;
    }
}