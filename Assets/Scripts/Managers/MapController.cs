using UnityEngine;
using UnityEngine.EventSystems;

public class MapController : MonoBehaviour, IDragHandler, IBeginDragHandler
{
    public RectTransform mapRect;
    public RectTransform zoomContainer;
    public float dragSpeed = 1f;
    Vector2 lastDragPosition;

    public Vector2 minPosition; // x: -400, y: -1000
    public Vector2 maxPosition; // x: 900, y: 3000

    public float zoomSpeed = 0.1f;
    public float minScale = 0.3f;
    public float maxScale = 1f;

    void Update()
    {
        HandleMouseZoom();
        HandleTouchZoom();
    }

    void HandleMouseZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.001f)
        {
            Vector2 screenPoint = Input.mousePosition;
            Zoom(scroll, screenPoint);
        }
    }

    void HandleTouchZoom()
    {
        if(Input.touchCount == 2)
        {
            Touch touch0 = Input.GetTouch(0);
            Touch touch1 = Input.GetTouch(1);

            Vector2 prevMid = (touch0.position - touch0.deltaPosition + touch1.position - touch1.deltaPosition) / 2f;
            Vector2 currMid = (touch0.position + touch1.position) / 2f;

            float prevDist = Vector2.Distance(touch0.position - touch0.deltaPosition, touch1.position - touch1.deltaPosition);
            float currDist = Vector2.Distance(touch0.position, touch1.position);
            float delta = currDist - prevDist;

            Zoom(delta * zoomSpeed * Time.deltaTime, currMid);
        }
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
}