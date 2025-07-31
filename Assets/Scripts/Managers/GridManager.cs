using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GridManager : MonoBehaviour
{
    #region Singleton
    public static GridManager Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    #endregion

    [SerializeField] GameObject parentObject;
    [SerializeField] GameObject gridPrefab;
    [SerializeField] Transform groundParent;
    public List<GameObject> grid = new List<GameObject>();
    int gridAmount = 5;
    int addition = 100;
    int gridHeight = 8;

    Vector3 originPosition;

    public void SetGridManagerPosition()
    {
        if(CityNavMeshSurfaceBuilder.Instance != null)
        {
            Vector3 surfaceCenter = CityNavMeshSurfaceBuilder.Instance.GetNavMeshSurfaceCenter();
            if(surfaceCenter != null)
            {
                RaycastHit hit;
                if(Physics.Raycast(surfaceCenter, Vector3.down, out hit, 500f))
                {
                    NavMeshHit navHit;
                    if(NavMesh.SamplePosition(hit.point, out navHit, 2f, NavMesh.AllAreas))
                        transform.position = navHit.position;
                }
            }
        }
    }

    Vector3 GetCenterOfChildren(Transform parent)
    {
        Vector3 total = Vector3.zero;
        int count = 0;

        foreach(Transform child in parent)
        {
            total += child.position;
            count++;
        }

        return total / count;
    }

    public void InitializeGrid()
    {
        //originPosition = GetCenterOfChildren(CityNavMeshSurfaceBuilder.Instance.transform);
        originPosition = CityNavMeshSurfaceBuilder.Instance.surface.navMeshData.sourceBounds.center;

        float halfGridSize = (gridAmount - 1) * addition / 2;

        for(int z = 0; z < gridAmount; z++)
        {
            for(int x = 0; x < gridAmount; x++)
            {
                float offsetX = -halfGridSize + x * addition;
                float offsetZ = -halfGridSize + z * addition;

                Vector3 currentPosition = new Vector3(originPosition.x + offsetX, parentObject.transform.position.y + gridHeight, originPosition.z + offsetZ);

                GameObject gridPrefabInstance = Instantiate(gridPrefab, currentPosition, Quaternion.identity);
                grid.Add(gridPrefabInstance);
                gridPrefabInstance.transform.parent = parentObject.transform;
            }
        }
    }
}