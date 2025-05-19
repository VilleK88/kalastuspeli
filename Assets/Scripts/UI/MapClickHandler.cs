using UnityEngine;
using Mapbox.Unity.Map;
using Mapbox.Utils;
using Mapbox.Unity.Utilities;
using UnityEngine.EventSystems;
using Unity.VisualScripting;

public class MapClickHandler : MonoBehaviour, IPointerClickHandler
{
    public AbstractMap map;
    public Transform player;
    public Transform waypoint2;


    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Clicked");
        Ray ray = Camera.main.ScreenPointToRay(eventData.position);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit))
        {
            Vector2d latLon = map.WorldToGeoPosition(hit.point);
            Vector3 newWorldPos = map.GeoToWorldPosition(latLon);
            newWorldPos.y = waypoint2.position.y;

            waypoint2.position = newWorldPos;
        }
    }

    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit))
            {
                Debug.Log("Clicked via raycast");
                Vector2d latLon = map.WorldToGeoPosition(hit.point);
                Vector3 newWorldPos = map.GeoToWorldPosition(latLon);
                newWorldPos.y = waypoint2.position.y;
                Debug.Log("Latlon: " + latLon);
                Debug.Log("New world position: " + newWorldPos);
                waypoint2.position = newWorldPos;
                map.UpdateMap(latLon, map.Zoom);
                player.position = map.GeoToWorldPosition(latLon);
            }
        }
    }
}