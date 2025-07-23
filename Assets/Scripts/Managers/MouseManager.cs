using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.InputSystem;

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
    public bool walking;
    private Vector3? targetPosition = null;
    int layerMask;

    float clickCooldown = 0.2f;
    float lastClickTime = 0f;

    [Header("Fishing Parameters")]
    public bool fishing;
    [SerializeField] Transform castPoint;
    [SerializeField] LineRenderer fishingLine;
    public BaitSO[] baits;
    public int selectedBaitIndex = 0;
    public GameObject currentBait;
    float launchSpeed = 90;
    float heightSpeedFactor = 2;
    [SerializeField] GameObject projectilePrefab;

    [Header("Trajectory Display")]
    public LineRenderer lineRenderer;
    public int linePoints = 175;
    public float timeIntervalinPoints = 0.01f;

    [Header("Input parameters")]
    [SerializeField] InputActionAsset inputActions;
    InputAction clickAction;

    void Start()
    {
        StartCoroutine(DelayedPlayerInitialization(1f)); // 0.5f
        layerMask = ~(1 << LayerMask.NameToLayer("Player"));
        lineRenderer = player.GetComponent<LineRenderer>();
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
        Vector2 screenPosition;

        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
            screenPosition = Touchscreen.current.primaryTouch.position.ReadValue();
        else if (Mouse.current != null)
            screenPosition = Mouse.current.position.ReadValue();
        else return;

        if (IsMarkerInfoPanelOpen() && fishing)
            return;

        if (Time.time - lastClickTime < clickCooldown)
            return;

        lastClickTime = Time.time;

        if (IsPointerOverUIObject(screenPosition))
            return;

        Ray ray = Camera.main.ScreenPointToRay(screenPosition);
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
            walking = true;
        }
    }

    void StopWalking()
    {
        targetPosition = null;
        playerAnim.SetBool("Walk", false);
        if (!agent.pathPending && agent.hasPath)
            agent.ResetPath();
        AudioManager.Instance.StopFootstepsSound();
        walking = false;
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

        StartCoroutine(DelayedLaunchProjectile(1.9f, markerTransform));
    }

    public void StopFishing()
    {
        playerAnim.Play("Fishing_Idle");
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

        fishingLine = castPoint.GetComponentInChildren<LineRenderer>();
    }

    IEnumerator DelayedLaunchProjectile(float duration, Transform markerTransform)
    {
        yield return new WaitForSeconds(duration);
        LaunchProjectile(markerTransform);
    }

    void LaunchProjectile(Transform markerTransform)
    {
        Vector3 launchPosition = activePlayerObject.transform.position + Vector3.up * 10;
        Transform childObject = markerTransform.GetChild(1);
        Renderer childRenderer = childObject.GetComponent<Renderer>();
        Vector3 targetCenter = childRenderer != null ? childRenderer.bounds.center : childObject.position;
        Vector3 direction = (targetCenter - launchPosition).normalized;

        float heightDifference = childObject.position.y - launchPosition.y;
        float adjustedSpeed = launchSpeed;

        if (heightDifference > 0)
            adjustedSpeed += heightDifference * heightSpeedFactor;

        GameObject prefabToThrow = baits[selectedBaitIndex].prefab;
        GameObject projectileInstance = Instantiate(prefabToThrow, launchPosition, Quaternion.LookRotation(direction));
        Rigidbody projectileRB = projectileInstance.GetComponent<Rigidbody>();
        projectileRB.linearVelocity = direction * adjustedSpeed;
        currentBait = projectileInstance;
    }

    void DrawTrajectory(Transform markerTransform)
    {
        Vector3 launchPosition = activePlayerObject.transform.position + Vector3.up * 10;
        Transform childObject = markerTransform.GetChild(1);
        Vector3 direction = (childObject.position - launchPosition).normalized;
        Vector3 startVelocity = direction * launchSpeed;
        lineRenderer.positionCount = linePoints;
        float time = 0;
        for (int i = 0; i < linePoints; i++)
        {
            // s = u*t + 1/2*g*t*t
            var x = (startVelocity.x * time) + (Physics.gravity.x / 2 * time * time);
            var y = (startVelocity.y * time) + (Physics.gravity.y / 2 * time * time);
            var z = (startVelocity.z * time) + (Physics.gravity.z / 2 * time * time);
            Vector3 point = new Vector3(x, y, z);
            lineRenderer.SetPosition(i, launchPosition + point);
            time += timeIntervalinPoints;
        }
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

    public Transform GetPlayerPosition()
    {
        return player.transform;
    }
}