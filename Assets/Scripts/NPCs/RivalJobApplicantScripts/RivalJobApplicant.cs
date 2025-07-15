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
    [HideInInspector] public RivalFishingState fishingState;

    public Animator anim;

    [Header("AI Navigation")]
    [HideInInspector] public NavMeshAgent agent;
    public float walkSpeed = 10f;

    public GameObject currentMarkerObject;
    public Marker currentMarker;
    public float currentDistance;

    float updateTimer = 0;
    float updateInterval = 0.2f;

    public Vector3 lastPosition;
    public float stuckTimer = 0;
    public float stuckThreshold = 0.05f;
    public float movementTolerance = 0.5f;

    public bool fishing;

    private void Awake()
    {
        idleState = new RivalIdleState(this);
        walkState = new RivalWalkState(this);
        fishingState = new RivalFishingState(this);
        agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        currentState = idleState;
        lastPosition = transform.position;
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
                if(!marker.markerOpen)
                {
                    closest = marker;
                    closestDistance = distance;
                }
            }
        }

        currentMarkerObject = closest.gameObject;
        currentDistance = closestDistance;
    }

    public void WarpToTarget(Vector3 target)
    {
        Debug.Log("Jumping to unreachable marker...");
        StartCoroutine(WarpCoroutine(target));
    }

    IEnumerator WarpCoroutine(Vector3 target)
    {
        agent.enabled = false;
        yield return new WaitForSeconds(0.5f);
        transform.position = target + Vector3.up * 1.5f;
        yield return null;
        agent.enabled = true;
        agent.Warp(target);
        FindClosestMarker();
    }

    public IEnumerator DestroyMarkerAndTransition()
    {
        if(currentMarker != null)
        {
            Debug.Log("Destroy the marker");
            currentMarker.DecreaseGridPrefabMarkerCount();
            Destroy(currentMarker.gameObject);
            MarkerManager.Instance.GenerateNewMarker();
        }

        yield return new WaitForSeconds(0.5f);
        currentMarkerObject = null;
        currentMarker = null;

        anim.SetBool("FishingIdle", false);
        FindClosestMarker();

        if (currentMarkerObject != null)
        {
            Debug.Log("Back to walk state");
            if (!agent.hasPath)
            {
                if (agent.enabled && agent.isOnNavMesh)
                {
                    Vector3 pointNearMarker = GetRandomPointNearMarker(currentMarkerObject.transform.position, 5, 10);
                    agent.SetDestination(pointNearMarker);
                }
            }
            anim.SetBool("Walk", true);
            fishing = false;
            currentState = walkState;
        }
        else
        {
            fishingState.idleStartTime = Time.time;
        }
    }

    public Vector3 GetRandomPointNearMarker(Vector3 markerPosition, float minDistance, float maxDistance)
    {
        Debug.Log("Get random point near marker");

        float currentDistance = Vector3.Distance(transform.position, currentMarkerObject.transform.position);
        if(currentDistance < 10)
        {
            Debug.Log("Already within fishing range. Staying put");
            return transform.position;
        }

        for (int i = 0; i < 30; i++)
        {
            float angle = Random.Range(0, 2 * Mathf.PI);
            float distance = Random.Range(minDistance, maxDistance);

            Vector3 direction = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle));
            Vector3 candidate = markerPosition + direction * distance;

            if(NavMesh.SamplePosition(candidate, out NavMeshHit hit, 50, NavMesh.AllAreas))
            {
                return hit.position;
            }
        }

        return markerPosition;
    }

    public void LookAtMarker(Transform markerTransform)
    {
        Vector3 targetPosition = markerTransform.position;
        Vector3 direction = new Vector3(targetPosition.x, agent.transform.position.y, targetPosition.z);
        transform.LookAt(direction);
    }

    private void OnTriggerEnter(Collider other)
    {
        Marker marker = other.GetComponentInParent<Marker>();
        if (marker != null && !marker.markerOpen)
        {
            fishing = true;
            currentMarker = marker;
        }
    }
}