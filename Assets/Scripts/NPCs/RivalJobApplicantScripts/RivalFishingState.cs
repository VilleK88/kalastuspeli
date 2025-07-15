using UnityEngine;

public class RivalFishingState : IRivalState
{
    RivalJobApplicant rival;
    public float idleStartTime;
    float maxIdleTime = 10;
    public bool coroutineRunning; // this makes sure that the coroutine only runs once

    public RivalFishingState(RivalJobApplicant thisRival)
    {
        this.rival = thisRival;
    }

    public void UpdateState()
    {
        if (Time.time - idleStartTime >= maxIdleTime)
        {
            if(!coroutineRunning)
                ToRivalWalkState();
        }
        else
        {
            rival.LookAtMarker(rival.currentMarkerObject.transform);
        }
    }

    public void ToRivalIdleState()
    {

    }

    public void ToRivalWalkState()
    {
        rival.StartCoroutine(rival.DestroyMarkerAndTransition());
        coroutineRunning = true;
    }

    public void ToRivalFishingState()
    {

    }
}