using UnityEngine;
using UnityEngine.AI;

public class Wrestler : MonoBehaviour
{
    [Header("Behaviour States")]
    [HideInInspector] public IWrestlerState currentState;
    [HideInInspector] public WIdleState idleState;
    [HideInInspector] public WWalkState walkState;

    public Animator anim;

    [Header("Waypoint Parameters")]
    public GameObject[] waypoints;
    public int waypointIndex = 0;

    [Header("AI Navigation")]
    public NavMeshAgent agent;
    public float walkSpeed = 10;

    float updateTimer = 0;
    float updateInterval = 0.2f;

    private void Awake()
    {
        idleState = new WIdleState(this);
        walkState = new WWalkState(this);
    }

    private void Start()
    {
        agent.SetDestination(waypoints[waypointIndex].transform.position);
        anim.SetBool("Walk", true);
        currentState = walkState;
    }

    private void Update()
    {
        updateTimer += Time.deltaTime;
        if(updateTimer >= updateInterval)
        {
            currentState.WUpdateState();
            updateTimer = 0;
        }
    }
}