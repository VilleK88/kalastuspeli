using UnityEngine;
using Mapbox.Unity.Map;
using Mapbox.Utils;
using Mapbox.Unity.Utilities;
using UnityEngine.EventSystems;
using Unity.VisualScripting;
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
    public float moveSpeed = 5f;
    private Vector3? targetPosition = null;

    [SerializeField] DirectionsFactory directionsFactory;

    void Start()
    {
        surface.GetComponent<NavMeshSurface>();
        StartCoroutine(DelayedAiInitialization(0.6f));
    }

    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit))
            {
                Vector2d latLon = map.WorldToGeoPosition(hit.point);
                Vector3 newWorldPos = map.GeoToWorldPosition(latLon);
                newWorldPos.y = waypoint2.position.y;
                waypoint2.position = newWorldPos;
                targetPosition = newWorldPos;
                //map.UpdateMap(latLon, map.Zoom);
                //player.position = map.GeoToWorldPosition(latLon);
            }
        }

        if(targetPosition.HasValue)
        {
            //player.position = Vector3.MoveTowards(player.position, targetPosition.Value, moveSpeed * Time.deltaTime);
            agent.destination = targetPosition.Value;

            if(Vector3.Distance(player.position, targetPosition.Value) < 0.1f)
            {
                targetPosition = null;
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