using System.Collections.Generic;
using UnityEngine;

public class GridPrefab : MonoBehaviour
{
    public int markerCount; // how many markers on this grid area

    [SerializeField] GameObject parentObject;
    public GameObject gridCellPrefab;
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
            GameObject gridCellInstance = Instantiate(gridCellPrefab, currentPosition, Quaternion.identity);
            grid.Add(gridCellInstance);
            gridCellInstance.transform.parent = parentObject.transform;
            gridCellInstance.transform.localScale = new Vector3(0.125f, 15, 0.125f);
            GridCellPrefab gridCellprefab = gridCellInstance.GetComponent<GridCellPrefab>();
            gridCellprefab.scaleX = 12.5f;
            gridCellprefab.scaleY = 15;
            gridCellprefab.scaleZ = 12.5f;
            transformStartX += addition;
        }
        transformStartX = startingValueX;
        transformStartZ -= addition;
    }

    public void DecreaseMarkerCount()
    {
        markerCount--;
    }

    /*private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, new Vector3(100f, 0.1f, 100f));
    }*/
}