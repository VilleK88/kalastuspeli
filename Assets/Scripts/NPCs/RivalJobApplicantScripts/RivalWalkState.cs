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

    }

    public void ToRivalWalkState()
    {

    }
}