using UnityEngine;
using UnityEngine.AI;

public class HiddenJobSeeker : MonoBehaviour
{
    [Header("Behaviour States")]
    [HideInInspector] public IHiddenJobSeekerState currentState;
    [HideInInspector] public HiddenIdleState idleState;
    [HideInInspector] public HiddenWalkState walkState;

    public Animator anim;

    [Header("Waypoint Parameters")]
    public GameObject[] waypoints;
    public int waypointIndex = 0;

    [Header("AI Navigation")]
    public NavMeshAgent agent;
    public float walkSpeed = 10;

    float updateTimer = 0;
    float updateInterval = 0.2f;

    public bool playerInRange;
    Transform playerTransform;
    float maxDistanceToPlayer = 25;

    private void Awake()
    {
        idleState = new HiddenIdleState(this);
        walkState = new HiddenWalkState(this);
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
        if (updateTimer >= updateInterval)
        {
            currentState.HiddenUpdateState();
            updateTimer = 0;

            if (CheckIfPlayerInRange())
            {
                Stop();
                LookAtPlayer();
            }
            else
                Resume();
        }
    }

    bool CheckIfPlayerInRange()
    {
        playerTransform = MouseManager.Instance.GetPlayerPosition();
        float distance = Vector3.Distance(transform.position, playerTransform.position);
        if(distance < maxDistanceToPlayer)
        {
            playerInRange = true;
            return true;
        }

        playerInRange = false;
        return false;
    }

    private void Stop()
    {
        agent.isStopped = true;
        anim.SetBool("Walk", false);
    }
    
    void Resume()
    {
        agent.isStopped = false;
        if (currentState == walkState)
            anim.SetBool("Walk", true);
    }

    void LookAtPlayer()
    {
        Vector3 targetPosition = playerTransform.position;
        Vector3 direction = new Vector3(targetPosition.x, agent.transform.position.y, targetPosition.z);
        transform.LookAt(direction);
    }
}