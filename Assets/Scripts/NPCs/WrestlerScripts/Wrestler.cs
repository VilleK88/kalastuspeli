using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Wrestler : MonoBehaviour
{
    [Header("Behaviour States")]
    [HideInInspector] public IWrestlerState currentState;
    [HideInInspector] public WIdleState idleState;
    [HideInInspector] public WWalkState walkState;

    public Animator anim;
    RuntimeAnimatorController ac;

    [Header("Waypoint Parameters")]
    public GameObject[] waypoints;
    public GameObject previousWaypoint;
    public GameObject currentWaypoint;

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
        ac = anim.runtimeAnimatorController;
        
        foreach(AnimationClip clip in ac.animationClips)
        {
            Debug.Log($"Clip {clip.name} duration: {clip.length} seconds");
        }

        currentState = idleState;
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