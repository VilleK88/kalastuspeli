using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.InputSystem;
using Unity.AI.Navigation;
using UnityEditor.ShaderGraph.Internal;

public class MouseManager : MonoBehaviour
{
    private void OnEnable()
    {
        inputActions.FindActionMap("Player").Enable();
        clickAction.performed += OnClickPerformed;
    }

    private void OnDisable()
    {
        inputActions.FindActionMap("Player").Disable();
        clickAction.performed -= OnClickPerformed;
    }

    #region Singleton
    public static MouseManager Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        clickAction = InputSystem.actions.FindAction("Player/Click");
    }
    #endregion

    [SerializeField] GameObject[] playerObjects;
    GameObject activePlayerObject;
    [SerializeField] GameObject player;
    Animator playerAnim;
    NavMeshAgent agent;
    public float moveSpeed = 5f;
    public float rotationSpeed = 10f;
    private Vector3? targetPosition = null;
    int layerMask;

    float clickCooldown = 0.2f;
    float lastClickTime = 0f;

    public bool fishing;
    [SerializeField] Transform castPoint;
    [SerializeField] LineRenderer fishingLine;

    public BaitSO[] baits;
    public int selectedBaitIndex = 0;

    public GameObject currentBait;
    float currentForce = 20f;

    [Header("Input parameters")]
    [SerializeField] InputActionAsset inputActions;
    InputAction clickAction;

    void Start()
    {
        StartCoroutine(DelayedPlayerInitialization(1f)); // 0.5f
        layerMask = ~(1 << LayerMask.NameToLayer("Player"));
    }

    void Update()
    {
        if (targetPosition.HasValue)
        {
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

        if(fishingLine != null)
            UpdateFishingLine();
    }

    void OnClickPerformed(InputAction.CallbackContext context)
    {
        if (IsMarkerInfoPanelOpen() && fishing)
            return;

        if (Time.time - lastClickTime < clickCooldown)
            return;

        lastClickTime = Time.time;

        if (IsPointerOverUIObject(Mouse.current.position.ReadValue()))
            return;

        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
        {
            if (IsPointerOverMarker(hit.collider.gameObject))
            {
                ClickMarker(hit.collider.gameObject);
                return;
            }

            Vector3 clickedPosition = hit.point;

            NavMeshPath path = new NavMeshPath();
            if (NavMesh.CalculatePath(agent.transform.position, clickedPosition, NavMesh.AllAreas, path) &&
                path.status == NavMeshPathStatus.PathComplete)
            {
                if (!agent.pathPending)
                {
                    agent.SetDestination(clickedPosition);
                    targetPosition = clickedPosition;
                    AudioManager.Instance.PlayFootstepsSound();
                }
            }
            else
                Debug.Log("Clicked position can't be reached");
        }
    }

    void Walk()
    {
        if(agent.velocity.sqrMagnitude > 0.01f)
        {
            Quaternion lookRotation = Quaternion.LookRotation(agent.velocity.normalized);
            player.transform.rotation = Quaternion.Slerp(player.transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
            playerAnim.SetBool("Walk", true);
        }
    }

    void StopWalking()
    {
        targetPosition = null;
        playerAnim.SetBool("Walk", false);
        if (!agent.pathPending && agent.hasPath)
            agent.ResetPath();
        AudioManager.Instance.StopFootstepsSound();
    }

    bool IsPointerOverUIObject(Vector2 screenPosition)
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = screenPosition;

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        foreach(RaycastResult result in results)
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
            marker.StartInteraction();
    }

    public void StartFishing(Transform markerTransform)
    {
        fishing = true;
        playerAnim.SetTrigger("Fishing_Cast");
        playerAnim.SetBool("Fishing_Idle", true);
        //ThrowLure();
        StartCoroutine(DelayedThrowLure(1.5f, markerTransform));
    }

    public void StopFishing()
    {
        playerAnim.SetBool("Fishing_Idle", false);
        Destroy(currentBait);
        StartCoroutine(DelayedStopFishing(1f));
    }

    IEnumerator DelayedStopFishing(float time)
    {
        yield return new WaitForSeconds(time);
        fishing = false;
    }

    public void LookAtMarker(Transform markerTransform)
    {
        Vector3 targetPosition = markerTransform.position;
        Vector3 direction = new Vector3(targetPosition.x, agent.transform.position.y, targetPosition.z);
        agent.transform.LookAt(direction);
        StartFishing(markerTransform);
    }

    void SetPlayerPosition()
    {
        if(CityNavMeshSurfaceBuilder.Instance != null)
        {
            Vector3 surfaceCenter = CityNavMeshSurfaceBuilder.Instance.GetNavMeshSurfaceCenter();
            if(surfaceCenter != null)
            {
                int mask = ~LayerMask.GetMask("Player");
                RaycastHit hit;
                if(Physics.Raycast(surfaceCenter + Vector3.up * 100f, Vector3.down, out hit, 1000f, mask))
                {
                    NavMeshHit navHit;
                    if(NavMesh.SamplePosition(hit.point, out navHit, 2f, NavMesh.AllAreas))
                    {
                        player.transform.position = navHit.position;
                        agent = player.GetComponent<NavMeshAgent>();
                        agent.Warp(navHit.position);
                        Debug.Log("Set player position");
                    }
                }
            }
        }
        else 
            Debug.LogError("CityNavMeshSurfaceBuilder not found");
    }

    void InitializePlayerGameObject()
    {
        SetPlayerPosition();
        player.SetActive(true);
        if (GameManager.Instance != null)
        {
            PlayerCharacter character = GameManager.Instance.character;
            string characterName = character.ToString();
            for (int i = 0; i < playerObjects.Length; i++)
            {
                if (playerObjects[i].name.Contains(characterName))
                {
                    activePlayerObject = playerObjects[i];
                    activePlayerObject.SetActive(true);
                    playerAnim = activePlayerObject.GetComponent<Animator>();
                    FindCastPointAndFishingLine();
                }
            }
        }
        //agent = player.GetComponent<NavMeshAgent>();
    }

    IEnumerator DelayedPlayerInitialization(float time)
    {
        yield return new WaitForSeconds(time);
        InitializePlayerGameObject();
    }

    void FindCastPointAndFishingLine()
    {
        castPoint = activePlayerObject.GetComponentsInChildren<Transform>(true)
            .FirstOrDefault(t => t.name == "castPoint");

        if (castPoint == null)
            Debug.LogWarning("castPoint not found in player's children.");
        else
            Debug.Log("castPoint found: " + castPoint.name);

        fishingLine = castPoint.GetComponentInChildren<LineRenderer>();
    }

    void ThrowLure(Transform markerTransform)
    {
        GameObject prefabToThrow = baits[selectedBaitIndex].prefab;
        GameObject lure = Instantiate(prefabToThrow, castPoint.position, castPoint.rotation);
        Rigidbody rb = lure.GetComponent<Rigidbody>();
        currentBait = lure;

        //Vector3 direction = (markerTransform.position - castPoint.position).normalized;
        //float distance = Vector3.Distance(markerTransform.position, castPoint.position);
        //float forceMultiplier = 3;
        //float adjustedForce = distance * forceMultiplier;
        //rb.AddForce(direction * adjustedForce, ForceMode.Impulse);

        Vector3 start = castPoint.position;
        Vector3 end = markerTransform.position;
        float height = 3f;
        Vector3 velocity = CalculateArcVelocity(start, end, height);
        rb.linearVelocity = velocity;
    }

    IEnumerator DelayedThrowLure(float time, Transform markerTransform)
    {
        yield return new WaitForSeconds(time);
        ThrowLure(markerTransform);
    }

    void UpdateFishingLine()
    {
        if (currentBait != null)
        {
            fishingLine.enabled = true;
            fishingLine.SetPosition(0, castPoint.position);
            fishingLine.SetPosition(1, currentBait.transform.position);
        }
        else
        {
            fishingLine.enabled = false;
        }
    }

    Vector3 CalculateArcVelocity(Vector3 start, Vector3 end, float arcHeight)
    {
        float gravity = Mathf.Abs(Physics.gravity.y);
        Vector3 direction = new Vector3(end.x - start.x, 0, end.z - start.z);
        float horizontalDistance = direction.magnitude;
        float verticalOffset = start.y - end.y;

        float timeUp = Mathf.Sqrt(2 * arcHeight / gravity);
        float timeDown = Mathf.Sqrt(2 * (arcHeight + verticalOffset) / gravity);
        float totalTime = timeUp + timeDown;

        Vector3 horizontalVelocity = direction / totalTime;
        float verticalVelocity = gravity * timeUp;

        return horizontalVelocity + Vector3.up * verticalVelocity;
    }
}