using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPC : MonoBehaviour
{
    [Header("Behaviour States")]
    [HideInInspector] public INPCState currentState;
    [HideInInspector] public IdleState idleState;
    [HideInInspector] public WalkState walkState;
    [HideInInspector] public PhoneState phoneState;
    [HideInInspector] public YawnState yawnState;

    [Header("NPC Parameters")]
    [SerializeField] NPCType npcType;
    [SerializeField] NPCData npcData;
    GameObject npcPrefab;

    public Animator anim;

    [Header("Waypoint Parameters")]
    public GameObject previousWaypoint;
    public GameObject currentWaypoint;

    [Header("AI Navigation")]
    public NavMeshAgent agent;
    public float walkSpeed = 10f;

    float updateTimer = 0;
    float updateInterval = 0.2f;

    private void Awake()
    {
        idleState = new IdleState(this);
        walkState = new WalkState(this);
        phoneState = new PhoneState(this);
        yawnState = new YawnState(this);
    }

    private void Start()
    {
        InitializeNPC();
        currentState = idleState;
        StartCoroutine(DelayedStartingStateChange(1f));
    }

    private void Update()
    {
        updateTimer += Time.deltaTime;
        if(updateTimer >= updateInterval)
        {
            currentState.UpdateState();
            updateTimer = 0;
        }

        //currentState.UpdateState();
    }

    void InitializeNPC()
    {
        NPCData[] allNPCData = Resources.LoadAll<NPCData>("NPCs");

        foreach(var data in allNPCData)
        {
            if(data.npcType == npcType)
            {
                npcData = data;
                break;
            }
        }

        if(npcData == null)
        {
            Debug.LogError($"NPCData not found for type {npcType}");
            return;
        }

        if(npcData.npcPrefab != null)
        {
            npcPrefab = Instantiate(npcData.npcPrefab, transform);
            npcPrefab.transform.localPosition = Vector3.zero;
            anim = npcPrefab.GetComponent<Animator>();
        }

        previousWaypoint = currentWaypoint;
        agent.speed = walkSpeed;
        currentState = walkState;
    }

    IEnumerator DelayedStartingStateChange(float time)
    {
        yield return new WaitForSeconds(time);
        agent.SetDestination(currentWaypoint.transform.position);
        anim.SetBool("Walk", true);
        currentState = walkState;
    }
}