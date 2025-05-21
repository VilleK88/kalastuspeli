using UnityEngine;
using Mapbox.Unity.Map;
using Mapbox.Utils;
using Mapbox.Unity.MeshGeneration.Factories;
using UnityEngine.AI;
using Unity.AI.Navigation;
using System.Collections;

public class MapClickHandler : MonoBehaviour
{
    public AbstractMap map;
    public Transform player;
    Animator playerAnim;
    NavMeshAgent agent;
    [SerializeField] NavMeshSurface surface;
    public Transform waypoint2;
    public float moveSpeed = 5f;
    public float rotationSpeed = 10f;
    private Vector3? targetPosition = null;

    [SerializeField] DirectionsFactory directionsFactory;

    public bool API_MapControl;

    void Start()
    {
        surface.GetComponent<NavMeshSurface>();
        playerAnim = player.GetComponentInChildren<Animator>();
        StartCoroutine(DelayedAiInitialization(0.6f));
    }

    void Update()
    {
        if (API_MapControl)
            APImouseControl();
        else
            MouseControl();
    }

    void APImouseControl()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                Vector2d latLon = map.WorldToGeoPosition(hit.point);
                Vector3 newWorldPos = map.GeoToWorldPosition(latLon);
                newWorldPos.y = waypoint2.position.y;
                waypoint2.position = newWorldPos;
                targetPosition = newWorldPos;
                Debug.Log("Clicked");
                Debug.Log("newWorldPos: " + newWorldPos);
            }
        }

        if (targetPosition.HasValue)
        {
            Debug.Log("agent.destination before: " + agent.destination);
            agent.destination = targetPosition.Value;
            Debug.Log("agent.destination after: " + agent.destination);

            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
            {
                if (!agent.hasPath || agent.velocity.sqrMagnitude < 0.01f)
                {
                    targetPosition = null;
                    playerAnim.SetBool("Walk", false);
                    agent.ResetPath();
                }
            }
            else
            {
                if (agent.velocity.sqrMagnitude > 0.01f)
                {
                    Quaternion lookRotation = Quaternion.LookRotation(agent.velocity.normalized);
                    player.rotation = Quaternion.Slerp(player.rotation, lookRotation, Time.deltaTime * rotationSpeed);
                    playerAnim.SetBool("Walk", true);
                }
            }
        }
    }

    void MouseControl()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if(Physics.Raycast(ray, out hit))
            {
                Vector3 clickedPosition = hit.point;
                clickedPosition.y = waypoint2.position.y;
                waypoint2.position = clickedPosition;
                targetPosition = clickedPosition;
                Debug.Log("Clicked");
            }
        }

        if(targetPosition.HasValue)
        {
            agent.destination = targetPosition.Value;

            if(!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
            {
                if(!agent.hasPath || agent.velocity.sqrMagnitude < 0.01f)
                {
                    targetPosition = null;
                    playerAnim.SetBool("Walk", false);
                    agent.ResetPath();
                }
            }
            else
            {
                if(agent.velocity.sqrMagnitude > 0.01f)
                {
                    Quaternion lookRotation = Quaternion.LookRotation(agent.velocity.normalized);
                    player.rotation = Quaternion.Slerp(player.rotation, lookRotation, Time.deltaTime * rotationSpeed);
                    playerAnim.SetBool("Walk", true);
                }
            }
        }
    }

    IEnumerator DelayedAiInitialization(float time)
    {
        yield return new WaitForSeconds(time);
        agent = player.GetComponent<NavMeshAgent>();
        surface.BuildNavMesh();
    }
}