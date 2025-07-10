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

    float updateTimer = 0;
    float updateInterval = 0.2f;

    public Vector3 lastPosition;
    public float stuckTimer = 0;
    public float stuckThreshold = 0.2f;
    public float movementTolerance = 0.5f;

    private void Awake()
    {
        idleState = new RivalIdleState(this);
        walkState = new RivalWalkState(this);
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
            CheckIfStuck();
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
                closest = marker;
                closestDistance = distance;
            }
        }

        currentMarkerObject = closest.gameObject;
        currentDistance = closestDistance;
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
        }
    }

    public void WarpToTarget(Vector3 target)
    {
        Debug.Log("Jumping to unreachable marker...");
        StartCoroutine(JumpOverBuilding(target));
    }

    IEnumerator JumpOverBuilding(Vector3 target)
    {
        agent.enabled = false;
        yield return new WaitForSeconds(0.5f);
        transform.position = target + Vector3.up * 1.5f;
        yield return null;
        agent.enabled = true;
        agent.Warp(target);
        FindClosestMarker();
    }

    void CheckIfStuck()
    {
        if (currentMarkerObject == null)
            return;

        float movedDistance = Vector3.Distance(transform.position, lastPosition);

        if(movedDistance < movementTolerance)
        {
            stuckTimer += Time.deltaTime;

            if(stuckTimer > stuckThreshold)
            {
                Debug.LogWarning("Rival stuck. Warping to marker.");
                WarpToTarget(currentMarkerObject.transform.position);
                stuckTimer = 0;
            }
        }
        else
        {
            stuckTimer = 0;
        }

            lastPosition = transform.position;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(lastPosition, 0.2f);
    }
}