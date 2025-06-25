using UnityEngine;

public class WalkState : INPCState
{
    NPC thisNPC;
    float waypointCounter = 2f;
    float wayPointMaxTime = 2f;

    public WalkState(NPC npc)
    {
        this.thisNPC = npc;
    }

    public void UpdateState()
    {
        float distanceToWaypoint = Vector3.Distance(thisNPC.transform.position, thisNPC.currentWaypoint.transform.position);
        if (distanceToWaypoint < 5f)
        {
            Waypoint waypoint = thisNPC.currentWaypoint.GetComponent<Waypoint>();
            GameObject[] waypoints = waypoint.waypoints;
            if(waypoints != null)
            {
                if(waypoints.Length > 1)
                {
                    while (true)
                    {
                        GameObject newWaypoint = SelectRandomWaypoint(waypoints);
                        if (newWaypoint != thisNPC.previousWaypoint)
                        {
                            thisNPC.previousWaypoint = thisNPC.currentWaypoint;
                            thisNPC.currentWaypoint = newWaypoint;
                            ToIdleState();
                            //SelectRandomIdleState();
                            break;
                        }
                    }
                }
                else
                {
                    thisNPC.currentWaypoint = thisNPC.previousWaypoint;
                    ToIdleState();
                    //SelectRandomIdleState();
                }
            }
        }
    }

    public void ToWalkState()
    {
    }

    public void ToIdleState()
    {
        thisNPC.anim.SetBool("Walk", false);
        thisNPC.currentState = thisNPC.idleState;
    }

    public void ToPhoneState()
    {
        thisNPC.anim.SetBool("Walk", false);
        thisNPC.anim.SetBool("Phone", true);
        thisNPC.currentState = thisNPC.phoneState;
    }

    public void ToYawnState()
    {

    }

    GameObject SelectRandomWaypoint(GameObject[] waypoints)
    {
        int randomIndex = Random.Range(0, waypoints.Length);
        return waypoints[randomIndex];
    }

    void SelectRandomIdleState()
    {
        int randomIndex = Random.Range(0, 2);
        if (randomIndex == 0)
            ToIdleState();
        else if (randomIndex == 1)
            ToPhoneState();
    }
}