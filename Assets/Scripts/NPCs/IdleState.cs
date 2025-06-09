using UnityEngine;

public class IdleState : INPCState
{
    NPC thisNPC;

    public IdleState(NPC npc)
    {
        this.thisNPC = npc;
    }

    public void UpdateState()
    {

    }

    public void ToIdleState()
    {
    }

    public void ToWanderState()
    {
        thisNPC.currentState = thisNPC.wanderState;
    }
}