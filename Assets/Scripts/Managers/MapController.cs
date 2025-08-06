using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class MapController : MonoBehaviour, IDragHandler, IBeginDragHandler
{
    public RectTransform mapRect;
    public RectTransform zoomContainer;

    public float dragSpeed = 1f;
    public float keyboardSpeed = 1000f;

    Vector2 lastDragPosition;

    public Vector2 minPosition; // x: 0, y: -1800
    public Vector2 maxPosition; // x: 0, y: 2350

    public float zoomSpeed = 0.1f;
    public float minScale = 0.3f;
    public float maxScale = 1f;

    [Header("Input Actions")]
    [SerializeField] InputActionAsset inputActions;
    InputAction moveAction;

    void OnEnable()
    {
        inputActions.FindActionMap("Player").Enable();
        moveAction = InputSystem.actions.FindAction("Player/Move");
    }

    void OnDisable()
    {
        inputActions.FindActionMap("Player").Disable();
    }

    void Update()
    {
        HandleKeyboardMovement();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        lastDragPosition = eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("Dragging");
        Vector2 dragDelta = eventData.position - lastDragPosition;
        lastDragPosition = eventData.position;

        Vector2 newPos = mapRect.anchoredPosition + dragDelta * dragSpeed;

        float minX = -400;
        float maxX = 400;

        newPos.x = Mathf.Clamp(newPos.x, minX, maxX);
        newPos.y = Mathf.Clamp(newPos.y, minPosition.y, maxPosition.y);

        mapRect.anchoredPosition = newPos;
    }

    void HandleKeyboardMovement()
    {
        Vector2 input = moveAction.ReadValue<Vector2>();
        if (input == Vector2.zero)
            return;

        Vector2 move = new Vector2(-input.x, -input.y) * keyboardSpeed * Time.deltaTime;
        Vector2 newPos = mapRect.anchoredPosition + move;

        float minX = -400;
        float maxX = 400;

        newPos.x = Mathf.Clamp(newPos.x, minX, maxX);
        newPos.y = Mathf.Clamp(newPos.y, minPosition.y, maxPosition.y);

        mapRect.anchoredPosition = newPos;
    }
}