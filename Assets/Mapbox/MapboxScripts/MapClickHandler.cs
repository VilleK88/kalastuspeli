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
    NavMeshAgent agent;
    [SerializeField] NavMeshSurface surface;
    public Transform waypoint2;
    private Vector3? targetPosition = null;

    [SerializeField] DirectionsFactory directionsFactory;

    public bool API_MapControl;

    void Start()
    {
        surface.GetComponent<NavMeshSurface>();
        StartCoroutine(DelayedAiInitialization(1f));
    }

    void Update()
    {
        APImouseControl();
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
                    agent.ResetPath();
                }
            }
            else
            {
                if (agent.velocity.sqrMagnitude > 0.01f)
                {
                    Quaternion lookRotation = Quaternion.LookRotation(agent.velocity.normalized);
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