using UnityEngine;

public class WWalkState : IWrestlerState
{
    Wrestler wrestler;

    public WWalkState(Wrestler thisWrestler)
    {
        this.wrestler = thisWrestler;
    }

    public void WUpdateState()
    {
        float distanceToWaypoint = Vector3.Distance(wrestler.transform.position, wrestler.waypoints[wrestler.waypointIndex].transform.position);
        if(distanceToWaypoint < 5)
        {
            ToWIdleState();
        }
    }

    public void ToWIdleState()
    {
        wrestler.anim.SetBool("Walk", false);
        wrestler.idleState.idleStartTime = Time.time;
        wrestler.currentState = wrestler.idleState;
    }

    public void ToWWalkState()
    {

    }
}