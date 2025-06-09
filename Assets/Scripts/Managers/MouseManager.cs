using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;
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
    [SerializeField] Transform player;
    Animator playerAnim;
    NavMeshAgent agent;
    [SerializeField] NavMeshSurface surface;
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
        InitializePlayerGameObject();
        surface.GetComponent<NavMeshSurface>();
        StartCoroutine(DelayedAiInitialization(0.6f));
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
            player.rotation = Quaternion.Slerp(player.rotation, lookRotation, Time.deltaTime * rotationSpeed);
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

    IEnumerator DelayedAiInitialization(float time)
    {
        yield return new WaitForSeconds(time);
        agent = player.GetComponent<NavMeshAgent>();
        surface.BuildNavMesh();
        MarkerManager.Instance.InitializeMarkers();
    }

    bool isPointerOverUIObject()
    {
        return IsPointerOverUIObject(Input.mousePosition);
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

        /*PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        foreach (RaycastResult result in results)
        {
            if (result.gameObject.GetComponent<UnityEngine.UI.Button>() != null)
                return true;
        }

        return false;*/
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

    public void StartFishing()
    {
        fishing = true;
        playerAnim.SetTrigger("Fishing_Cast");
        playerAnim.SetBool("Fishing_Idle", true);
        //ThrowLure();
    }

    public void StopFishing()
    {
        playerAnim.SetBool("Fishing_Idle", false);
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
    }

    void InitializePlayerGameObject()
    {
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
                    //FindCastPointAndFishingLine();
                }
            }
        }
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

    void ThrowLure()
    {
        GameObject prefabToThrow = baits[selectedBaitIndex].prefab;
        GameObject lure = Instantiate(prefabToThrow, castPoint.position, castPoint.rotation);
        Rigidbody rb = lure.GetComponent<Rigidbody>();
        currentBait = lure;
        rb.AddForce(castPoint.forward * currentForce, ForceMode.Impulse);
    }
}