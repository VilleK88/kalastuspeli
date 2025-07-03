using UnityEngine;

public class IdleState : INPCState
{
    NPC thisNPC;
    public float idleStartTime;
    float maxIdleTime = 2;

    public IdleState(NPC npc)
    {
        this.thisNPC = npc;
    }

    public void UpdateState()
    {
        if(Time.time - idleStartTime >= maxIdleTime)
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
        thisNPC.currentState = thisNPC.walkState;
    }

    public void ToPhoneState()
    {

    }

    public void ToYawnState()
    {

    }
}