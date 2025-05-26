using UnityEngine;
using Mapbox.Examples;
using Mapbox.Utils;

public class EventPointer : MonoBehaviour
{
    [SerializeField] float rotationSpeed = 50f;
    [SerializeField] float amplitude = 2f;
    [SerializeField] float frequency = 0.5f;
    float baseHeight = 8f;

    LocationStatus playerLocation;
    public Vector2d eventPos;
    public int eventID;
    MenuUIManager menuUIManager;

    void Start()
    {
        menuUIManager = GameObject.Find("MapUIcanvas").GetComponent<MenuUIManager>();
    }

    void Update()
    {
        FloatAndRotatePointer();
    }

    void FloatAndRotatePointer()
    {
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
        float floatOffset = Mathf.Sin(Time.fixedTime * Mathf.PI * frequency) * amplitude;
        transform.position = new Vector3(transform.position.x, baseHeight + floatOffset, transform.position.z);
    }

    private void OnMouseDown()
    {
        playerLocation = GameObject.Find("MapUIcanvas").GetComponent<LocationStatus>();
        var currentPlayerLocation = new GeoCoordinatePortable.GeoCoordinate(playerLocation.GetLocationLat(), playerLocation.GetLocationLon());
        var eventLocation = new GeoCoordinatePortable.GeoCoordinate(eventPos[0], eventPos[1]);
        var distance = currentPlayerLocation.GetDistanceTo(eventLocation);
        Debug.Log("Distance is: " + distance);
        if(distance < 70)
        {
            menuUIManager.DisplayStartEventPanel();
        }
        else
        {
            menuUIManager.DisplayUserNotInRangePanel();
        }
    }
}