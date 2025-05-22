using UnityEngine;
using Mapbox.Unity.Map;
using Mapbox.Utils;
using Mapbox.Unity.MeshGeneration.Factories;
using UnityEngine.AI;
using Unity.AI.Navigation;
using System.Collections;

public class MouseManager : MonoBehaviour
{
    public Transform player;
    Animator playerAnim;
    NavMeshAgent agent;
    [SerializeField] NavMeshSurface surface;
    public float moveSpeed = 5f;
    public float rotationSpeed = 10f;
    private Vector3? targetPosition = null;


    void Start()
    {
        surface.GetComponent<NavMeshSurface>();
        playerAnim = player.GetComponentInChildren<Animator>();
        StartCoroutine(DelayedAiInitialization(0.6f));
    }

    void Update()
    {
        MouseControl();
    }

    void MouseControl()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                Vector3 clickedPosition = hit.point;

                NavMeshPath path = new NavMeshPath();
                if(NavMesh.CalculatePath(agent.transform.position, clickedPosition, NavMesh.AllAreas, path) &&
                    path.status == NavMeshPathStatus.PathComplete)
                {
                    targetPosition = clickedPosition;
                    Debug.Log("Clicked");
                }
                else
                {
                    Debug.Log("Clicked position can't be reached");
                }
            }
        }

        if (targetPosition.HasValue)
        {
            agent.destination = targetPosition.Value;

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

    IEnumerator DelayedAiInitialization(float time)
    {
        yield return new WaitForSeconds(time);
        agent = player.GetComponent<NavMeshAgent>();
        surface.BuildNavMesh();
    }
}