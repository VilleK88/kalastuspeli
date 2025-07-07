using UnityEngine;

public class WIdleState : IWrestlerState
{
    Wrestler wrestler;
    public float idleStartTime;
    public float maxIdleTime = 2;

    public WIdleState(Wrestler thisWrestler)
    {
        this.wrestler = thisWrestler;
    }

    public void WUpdateState()
    {
        if(Time.time - idleStartTime >= maxIdleTime)
        {
            ToWWalkState();
        }
    }

    public void ToWIdleState()
    {

    }

    public void ToWWalkState()
    {
        if (wrestler.waypointIndex < wrestler.waypoints.Length - 1)
            wrestler.waypointIndex++;
        else
            wrestler.waypointIndex = 0;

        wrestler.agent.SetDestination(wrestler.waypoints[wrestler.waypointIndex].transform.position);
        wrestler.anim.SetBool("Walk", true);
        wrestler.currentState = wrestler.walkState;
    }
}