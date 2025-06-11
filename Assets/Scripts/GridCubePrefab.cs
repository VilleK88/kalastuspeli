using UnityEngine;

public class GridCubePrefab : MonoBehaviour
{
    public float scaleX, scaleY, scaleZ;
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, new Vector3(scaleX, scaleY, scaleZ));
    }
}