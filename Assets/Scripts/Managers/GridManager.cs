using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public List<GameObject> grid = new List<GameObject>();
    int gridAmount = 5;
    int transformStartX = -165;
    int startingValueX;
    int transformStartZ = 175;
    int addition = 100;
    int gridHeight = 8;

    Vector3 originPosition;

    private void Start()
    {
        //transform.position = GetCenterOfChildren(CityNavMeshSurfaceBuilder.Instance.transform);
        originPosition = GetCenterOfChildren(CityNavMeshSurfaceBuilder.Instance.transform);
        //startingValueX = transformStartX;
        InitializeGrid();
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

    void InitializeGrid()
    {
        /*for(int i = 0; i < gridAmount; i++)
        {
            InitializeGridRow();
        }*/

        float halfGridSize = (gridAmount - 1) * addition / 2;

        for(int z = 0; z < gridAmount; z++)
        {
            for(int x = 0; x < gridAmount; x++)
            {
                float offsetX = -halfGridSize + x * addition;
                float offsetZ = -halfGridSize + z * addition;

                Vector3 currentPosition = new Vector3(originPosition.x + offsetX, gridHeight, originPosition.z + offsetZ);

                GameObject gridPrefabInstance = Instantiate(gridPrefab, currentPosition, Quaternion.identity);
                grid.Add(gridPrefabInstance);
                gridPrefabInstance.transform.parent = parentObject.transform;
            }
        }
    }

    void InitializeGridRow()
    {
        for (int i = 0; i < gridAmount; i++)
        {
            Vector3 currentPosition = new Vector3(transformStartX, gridHeight, transformStartZ);
            GameObject gridPrefabInstance = Instantiate(gridPrefab, currentPosition, Quaternion.identity);
            grid.Add(gridPrefabInstance);
            gridPrefabInstance.transform.parent = parentObject.transform;
            transformStartX += addition;
        }
        transformStartX = startingValueX;
        transformStartZ -= addition;
    }
}