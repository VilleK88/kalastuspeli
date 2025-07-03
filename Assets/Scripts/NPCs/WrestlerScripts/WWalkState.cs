using UnityEngine;

public class WWalkState : IWrestlerState
{
    Wrestler wrestler;

    public WWalkState(Wrestler thisWrestler)
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