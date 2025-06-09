using UnityEngine;

public class WanderState : INPCState
{
    NPC thisNPC;
    float waypointCounter = 2f;
    float wayPointMaxTime = 2f;

    public WanderState(NPC npc)
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
                            break;
                        }
                    }
                }
                else
                {
                    thisNPC.currentWaypoint = thisNPC.previousWaypoint;
                }
            }
            thisNPC.anim.SetBool("Walk", false);
            waypointCounter = 0;
        }

        if(waypointCounter < wayPointMaxTime)
        {
            waypointCounter += Time.deltaTime;
        }
        else
        {
            thisNPC.agent.SetDestination(thisNPC.currentWaypoint.transform.position);
            thisNPC.anim.SetBool("Walk", true);
            //Debug.Log("Distance to waypoint: " + distanceToWaypoint);
        }
    }

    public void ToWanderState()
    {
    }

    public void ToIdleState()
    {
        thisNPC.currentState = thisNPC.idleState;
    }

    GameObject SelectRandomWaypoint(GameObject[] waypoints)
    {
        int randomIndex = Random.Range(0, waypoints.Length);
        return waypoints[randomIndex];
    }
}