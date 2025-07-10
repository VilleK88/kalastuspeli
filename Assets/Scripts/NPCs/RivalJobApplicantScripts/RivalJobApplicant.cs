using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RivalJobApplicant : MonoBehaviour
{
    [Header("Behaviour States")]
    [HideInInspector] public IRivalState currentState;
    [HideInInspector] public RivalIdleState idleState;
    [HideInInspector] public RivalWalkState walkState;

    public Animator anim;

    [Header("AI Navigation")]
    [HideInInspector] public NavMeshAgent agent;
    public float walkSpeed = 10f;

    public GameObject currentMarkerObject;
    public float currentDistance;
    public bool currentMarker;

    float detectionRadius = 100;
    float stopDistance = 1.5f;

    float updateTimer = 0;
    float updateInterval = 0.2f;

    private void Awake()
    {
        idleState = new RivalIdleState(this);
        walkState = new RivalWalkState(this);
    }

    private void Start()
    {
        StartCoroutine(DelayedStart(3));
        currentState = walkState;
        agent.SetDestination(currentMarkerObject.transform.position);
        anim.SetBool("Walk", true);
    }

    private void Update()
    {
        updateTimer += Time.deltaTime;
        if (updateTimer >= updateInterval)
        {
            if(currentMarkerObject == null && agent != null)
            {
                Debug.Log("Find closest marker object");
                FindClosestMarker();
            }

            currentState.UpdateState();
            updateTimer = 0;
        }
    }

    IEnumerator DelayedStart(float time)
    {
        yield return new WaitForSeconds(3);
        agent = GetComponent<NavMeshAgent>();
        //FindClosestMarker();
        yield return null;
    }

    public void FindClosestMarker()
    {
        var foundMarkers = FindObjectsByType<Marker>(FindObjectsSortMode.None);

        if(foundMarkers.Length == 0)
        {
            currentMarkerObject = null;
            Debug.LogWarning("No markers found in the scene.");
            return;
        }

        Marker closest = null;
        float closestDistance = Mathf.Infinity;

        foreach(var marker in foundMarkers)
        {
            float distance = Vector3.Distance(transform.position, marker.transform.position);
            if(distance < closestDistance)
            {
                closest = marker;
                closestDistance = distance;
            }
        }

        currentMarkerObject = closest.gameObject;
        currentDistance = closestDistance;

        //agent.SetDestination(currentMarkerObject.transform.position);
        currentMarker = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        Marker marker = other.GetComponentInParent<Marker>();
        if(marker != null)
        {
            marker.DecreaseGridPrefabMarkerCount();
            Destroy(marker.gameObject);
            MarkerManager.Instance.GenerateNewMarker();
            currentMarkerObject = null;
            walkState.ToRivalIdleState();
        }
    }
}