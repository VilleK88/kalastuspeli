using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;

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
    int layerMask;

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
        layerMask = ~(1 << LayerMask.NameToLayer("Player"));
    }

    void Update()
    {
        if(!IsMarkerInfoPanelOpen())
            MouseControl();
    }

    void MouseControl()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (IsPointerOverUIObject())
                return;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
            {
                if(IsPointerOverMarker(hit.collider.gameObject))
                {
                    ClickMarker(hit.collider.gameObject);
                    return;
                }

                Vector3 clickedPosition = hit.point;

                NavMeshPath path = new NavMeshPath();
                if(NavMesh.CalculatePath(agent.transform.position, clickedPosition, NavMesh.AllAreas, path) &&
                    path.status == NavMeshPathStatus.PathComplete)
                {
                    if(targetPosition == null)
                        AudioManager.Instance.PlayFootstepsSound();
                    targetPosition = clickedPosition;
                }
                else
                    Debug.Log("Clicked position can't be reached");
            }
        }

        if (targetPosition.HasValue)
        {
            agent.destination = targetPosition.Value;

            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
            {
                if (!agent.hasPath || agent.velocity.sqrMagnitude < 0.01f)
                    StopWalking();
            }
            else
            {
                if (agent.velocity.sqrMagnitude > 0.01f)
                    Walk();
            }
        }
    }

    void Walk()
    {
        Quaternion lookRotation = Quaternion.LookRotation(agent.velocity.normalized);
        player.rotation = Quaternion.Slerp(player.rotation, lookRotation, Time.deltaTime * rotationSpeed);
        playerAnim.SetBool("Walk", true);
    }

    void StopWalking()
    {
        targetPosition = null;
        playerAnim.SetBool("Walk", false);
        agent.ResetPath();
        AudioManager.Instance.StopFootstepsSound();
    }

    IEnumerator DelayedAiInitialization(float time)
    {
        yield return new WaitForSeconds(time);
        agent = player.GetComponent<NavMeshAgent>();
        surface.BuildNavMesh();
    }

    bool IsPointerOverUIObject()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        foreach (RaycastResult result in results)
        {
            if (result.gameObject.GetComponent<UnityEngine.UI.Button>() != null)
                return true;
        }

        return false;
    }

    bool IsMarkerInfoPanelOpen()
    {
        if (MarkerUI.Instance.open)
            return true;
        return false;
    }

    bool IsPointerOverMarker(GameObject hitObject)
    {
        if (hitObject.layer == LayerMask.NameToLayer("Marker"))
            return true;
        return false;
    }

    void ClickMarker(GameObject hitObject)
    {
        Marker marker = hitObject.GetComponentInParent<Marker>();
        if(marker != null)
        {
            marker.StartInteraction();
        }
    }
}