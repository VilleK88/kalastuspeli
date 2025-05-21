using UnityEngine;
using Mapbox.Utils;

public class CoordinateConverter : MonoBehaviour
{
    public City city;
    private Vector2d latLonHelsinki = new Vector2d(60.1709275148649, 24.9397556919865);
    public Vector2d centerLatLon;
    public Vector3 centerWorldPos = Vector3.zero;

    private void Start()
    {
        if(city == City.Helsinki)
        {
            centerLatLon = latLonHelsinki;
        }
    }

    public Vector3 LatLonToUnity(Vector2d latLon)
    {
        float metersPerDegreeLat = 111132f;
        float metersPerDegreeLon = 111320f * Mathf.Cos((float)(centerLatLon.x * Mathf.Deg2Rad));

        double deltaLat = latLon.x - centerLatLon.x;
        double deltaLon = latLon.y - centerLatLon.y;

        float xOffset = (float)(deltaLon * metersPerDegreeLat);
        float zOffset = (float)(deltaLat * metersPerDegreeLon);

        return centerWorldPos + new Vector3(xOffset, 0, zOffset);
    }
}

public enum City
{
    Helsinki,
    Turku,
    Tampere
}