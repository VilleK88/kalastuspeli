using UnityEngine;

public class PhoneState : INPCState
{
    NPC thisNPC;

    public PhoneState(NPC npc)
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
        thisNPC.currentState = thisNPC.walkState;
    }
}