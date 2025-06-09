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

    public Vector2 minPosition; // x: -400, y: -1000
    public Vector2 maxPosition; // x: 900, y: 3000

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
        HandleMouseZoom();
        HandleTouchZoom();
        HandleKeyboardMovement();
    }

    void HandleMouseZoom()
    {
        float scroll = Mouse.current.scroll.ReadValue().y * 0.1f;
        if (Mathf.Abs(scroll) > 0.001f)
        {
            Vector2 screenPoint = Input.mousePosition;
            Zoom(scroll, screenPoint);
        }
    }

    void HandleTouchZoom()
    {
        if (Touchscreen.current == null || Touchscreen.current.touches.Count < 2)
            return;

        var touches = Touchscreen.current.touches;

        if (!touches[0].isInProgress || !touches[1].isInProgress)
            return;

        Vector2 touch0Prev = touches[0].position.ReadValue() - touches[0].delta.ReadValue();
        Vector2 touch1Prev = touches[1].position.ReadValue() - touches[1].delta.ReadValue();

        Vector2 prevMid = (touch0Prev + touch1Prev) / 2f;
        Vector2 currMid = (touches[0].position.ReadValue() + touches[1].position.ReadValue() / 2f);


        float prevDist = Vector2.Distance(touch0Prev, touch1Prev);
        float currDist = Vector2.Distance(touches[0].position.ReadValue(), touches[1].position.ReadValue());

        float delta = currDist - prevDist;

        Zoom(delta * zoomSpeed * Time.deltaTime, currMid);
    }

    void Zoom(float amount, Vector2 screenPoint)
    {
        float currentScale = mapRect.localScale.x;
        float newScale = Mathf.Clamp(currentScale + amount, minScale, maxScale);
        float scaleDelta = newScale / currentScale;

        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(zoomContainer, screenPoint, null, out localPoint);

        Vector2 pivotOffset = mapRect.anchoredPosition - localPoint;
        Vector2 newPosition = localPoint + pivotOffset * scaleDelta;

        mapRect.localScale = new Vector3(newScale, newScale, 1f);
        mapRect.anchoredPosition = newPosition;
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

        newPos.x = Mathf.Clamp(newPos.x, minPosition.x, maxPosition.x);
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

        newPos.x = Mathf.Clamp(newPos.x, minPosition.y, maxPosition.x);
        newPos.y = Mathf.Clamp(newPos.y, minPosition.y, maxPosition.y);

        mapRect.anchoredPosition = newPos;
    }
}