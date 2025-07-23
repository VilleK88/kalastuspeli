using UnityEngine;

public class HiddenWalkState : IHiddenJobSeekerState
{
    HiddenJobSeeker hiddenJobSeeker;

    public HiddenWalkState(HiddenJobSeeker thisHiddenJobSeeker)
    {
        this.hiddenJobSeeker = thisHiddenJobSeeker;
    }

    public void HiddenUpdateState()
    {
        float distanceToWaypoint = Vector3.Distance(hiddenJobSeeker.transform.position, hiddenJobSeeker.waypoints[hiddenJobSeeker.waypointIndex].transform.position);
        if (distanceToWaypoint < 5)
        {
            if (!hiddenJobSeeker.playerInRange)
                ToHiddenIdleState();
        }
    }

    public void ToHiddenIdleState()
    {
        hiddenJobSeeker.idleState.idleStartTime = Time.time;
        hiddenJobSeeker.anim.SetBool("Walk", false);
        hiddenJobSeeker.currentState = hiddenJobSeeker.idleState;
    }

    public void ToHiddenWalkState()
    {

    }
}