using UnityEngine;

public class WIdleState : IWrestlerState
{
    Wrestler wrestler;

    public WIdleState(Wrestler thisWrestler)
    {
        this.wrestler = thisWrestler;
    }

    public void WUpdateState()
    {

    }

    public void ToWIdleState()
    {

    }

    public void ToWWalkState()
    {

    }
}