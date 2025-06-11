using UnityEngine;

public class GridCubePrefab : MonoBehaviour
{
    [SerializeField] LayerMask obstacleMask;
    public float scaleX, scaleY, scaleZ;

    private void Start()
    {
        Vector3 checkAreaSize = new Vector3(scaleX, scaleY, scaleZ);
        Vector3 center = transform.position + Vector3.up * (checkAreaSize.y / 2);

        if (Physics.CheckBox(center, checkAreaSize / 2, Quaternion.identity, obstacleMask))
            gameObject.SetActive(false);
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, new Vector3(scaleX, scaleY, scaleZ));
    }
}