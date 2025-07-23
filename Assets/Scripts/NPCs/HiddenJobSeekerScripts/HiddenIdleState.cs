using UnityEngine;

public class HiddenIdleState : IHiddenJobSeekerState
{
    HiddenJobSeeker hiddenJobSeeker;
    public float idleStartTime;
    float maxIdleTime = 2;

    public HiddenIdleState(HiddenJobSeeker thisHiddenJobSeeker)
    {
        this.hiddenJobSeeker = thisHiddenJobSeeker;
    }

    public void HiddenUpdateState()
    {
        if (Time.time - idleStartTime >= maxIdleTime)
        {
            if(!hiddenJobSeeker.playerInRange)
                ToHiddenWalkState();
        }
    }

    public void ToHiddenIdleState()
    {

    }

    public void ToHiddenWalkState()
    {
        if (hiddenJobSeeker.waypointIndex < hiddenJobSeeker.waypoints.Length - 1)
            hiddenJobSeeker.waypointIndex++;
        else
            hiddenJobSeeker.waypointIndex = 0;

        hiddenJobSeeker.agent.SetDestination(hiddenJobSeeker.waypoints[hiddenJobSeeker.waypointIndex].transform.position);
        hiddenJobSeeker.anim.SetBool("Walk", true);
        hiddenJobSeeker.currentState = hiddenJobSeeker.walkState;
    }
}