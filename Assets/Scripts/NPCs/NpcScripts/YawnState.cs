using UnityEngine;

public class YawnState : INPCState
{
    NPC thisNPC;
    float waypointCounter = 0;
    float wayPointMaxTime = 4;

    public YawnState(NPC npc)
    {
        this.thisNPC = npc;
    }

    public void UpdateState()
    {
        if (waypointCounter < wayPointMaxTime)
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
        thisNPC.anim.SetBool("Yawn", false);
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