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

    private void Start()
    {
        startingValueX = transformStartX;
        InitializeGrid();
    }

    void InitializeGrid()
    {
        for(int i = 0; i < gridAmount; i++)
        {
            InitializeGridRow();
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