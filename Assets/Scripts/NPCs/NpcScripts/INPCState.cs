public interface INPCState
{
    void UpdateState();
    void ToIdleState();
    void ToWalkState();
    void ToPhoneState();
    void ToYawnState();
}