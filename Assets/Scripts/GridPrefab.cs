using System.Collections.Generic;
using UnityEngine;

public class GridPrefab : MonoBehaviour
{
    public int markerCount; // how many markers on this grid area

    [SerializeField] GameObject parentObject;
    public GameObject gridCubePrefab;
    public List<GameObject> grid = new List<GameObject>();
    int gridAmount = 8;
    float transformStartX = -43.75f;
    float startingValueX;
    float transformStartZ = 43.75f;
    float addition = 12.5f;

    private void Start()
    {
        startingValueX = transformStartX;
        InitializeGrid();
    }

    void InitializeGrid()
    {
        for (int i = 0; i < gridAmount; i++)
        {
            InitializeGridRow();
        }
    }

    void InitializeGridRow()
    {
        for (int i = 0; i < gridAmount; i++)
        {
            Vector3 currentPosition = new Vector3(parentObject.transform.position.x + transformStartX, parentObject.transform.position.y, parentObject.transform.position.z + transformStartZ);
            GameObject gridPrefabInstance = Instantiate(gridCubePrefab, currentPosition, Quaternion.identity);
            grid.Add(gridPrefabInstance);
            gridPrefabInstance.transform.parent = parentObject.transform;
            gridPrefabInstance.transform.localScale = new Vector3(0.125f, 15, 0.125f);
            GridCubePrefab gridCubePrefabInstance = gridPrefabInstance.GetComponent<GridCubePrefab>();
            gridCubePrefabInstance.scaleX = 12.5f;
            gridCubePrefabInstance.scaleY = 15;
            gridCubePrefabInstance.scaleZ = 12.5f;
            transformStartX += addition;
        }
        transformStartX = startingValueX;
        transformStartZ -= addition;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, new Vector3(100f, 0.1f, 100f));
    }
}