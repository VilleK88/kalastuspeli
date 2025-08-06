using UnityEngine;

public class Waypoint : MonoBehaviour
{
    public GameObject[] waypoints;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, 1);
        if(waypoints != null)
            DrawPaths();
    }

    void DrawPaths()
    {
        Gizmos.color = Color.yellow;
        for(int i = 0; i < waypoints.Length; i++)
        {
            if (waypoints[i] != null)
                Gizmos.DrawLine(transform.position, waypoints[i].transform.position);
        }
    }
}