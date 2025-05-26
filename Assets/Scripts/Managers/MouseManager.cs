using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;
using System.Collections;

public class MouseManager : MonoBehaviour
{
    [SerializeField] GameObject[] playerObjects;
    [SerializeField] Transform player;
    Animator playerAnim;
    NavMeshAgent agent;
    [SerializeField] NavMeshSurface surface;
    public float moveSpeed = 5f;
    public float rotationSpeed = 10f;
    private Vector3? targetPosition = null;


    void Start()
    {
        if(GameManager.Instance != null)
        {
            PlayerCharacter character = GameManager.Instance.character;
            string characterName = character.ToString();
            for(int i = 0; i < playerObjects.Length; i++)
            {
                if (playerObjects[i].name.Contains(characterName))
                {
                    playerObjects[i].SetActive(true);
                    playerAnim = playerObjects[i].GetComponent<Animator>();
                }
            }
        }
        surface.GetComponent<NavMeshSurface>();
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
                    if(targetPosition == null)
                        AudioManager.Instance.PlayFootstepsSound();
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
                    AudioManager.Instance.StopFootstepsSound();
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