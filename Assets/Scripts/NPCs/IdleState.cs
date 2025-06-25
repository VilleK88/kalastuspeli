using UnityEngine;

public class IdleState : INPCState
{
    NPC thisNPC;
    float waypointCounter = 0;
    float wayPointMaxTime = 2;

    public IdleState(NPC npc)
    {
        this.thisNPC = npc;
    }

    public void UpdateState()
    {
        if(waypointCounter < wayPointMaxTime)
        {
            waypointCounter += Time.deltaTime;
        }
        else
        {
            ToWalkState();
        }
    }

    public void ToIdleState()
    {
    }

    public void ToWalkState()
    {
        thisNPC.agent.SetDestination(thisNPC.currentWaypoint.transform.position);
        thisNPC.anim.SetBool("Walk", true);
        waypointCounter = 0;
        thisNPC.currentState = thisNPC.walkState;
    }

    public void ToPhoneState()
    {

    }

    public void ToYawnState()
    {

    }
}