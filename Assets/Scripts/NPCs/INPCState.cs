using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface INPCState
{
    void UpdateState();
    void ToIdleState();
    void ToWanderState();
}