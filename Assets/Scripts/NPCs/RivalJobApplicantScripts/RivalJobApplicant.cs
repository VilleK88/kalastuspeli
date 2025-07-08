using UnityEngine;
using UnityEngine.AI;

public class RivalJobApplicant : MonoBehaviour
{
    [Header("Behaviour States")]
    [HideInInspector] public INPCState currentState;
    [HideInInspector] public RivalIdleState idleState;
    [HideInInspector] public RivalWalkState walkState;

    public Animator anim;

    [Header("AI Navigation")]
    public NavMeshAgent agent;
    public float walkSpeed = 10f;

    float updateTimer = 0;
    float updateInterval = 0.2f;

    private void Awake()
    {
        idleState = new RivalIdleState(this);
        walkState = new RivalWalkState(this);
    }

    private void Start()
    {
        
    }

    private void Update()
    {
        updateTimer += Time.deltaTime;
        if (updateTimer >= updateInterval)
        {
            currentState.UpdateState();
            updateTimer = 0;
        }
    }
}