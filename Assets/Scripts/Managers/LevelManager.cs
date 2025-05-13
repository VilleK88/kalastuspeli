using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public GameObject[] buildingPrefabs;
    public int buildingCount = 50;
    public float minDistanceBetweenBuildings = 10f;

    private Terrain terrain;

    void Start()
    {
        terrain = Terrain.activeTerrain;
        if(terrain == null)
        {
            Debug.Log("Terrain not found in scene.");
            return;
        }

        GenerateBuildings();
    }

    void GenerateBuildings()
    {
        for(int i = 0; i < buildingCount; i++)
        {
            Vector3 position = GetRandomPositionOnTerrain();

            if(IsPositionValid(position))
            {
                GameObject prefab = buildingPrefabs[Random.Range(0, buildingPrefabs.Length)];
                Instantiate(prefab, position, Quaternion.identity);
            }
            else
            {
                i--;
            }
        }
    }

    Vector3 GetRandomPositionOnTerrain()
    {
        float terrainWidth = terrain.terrainData.size.x;
        float terrainLength = terrain.terrainData.size.y;

        float x = Random.Range(0, terrainWidth);
        float z = Random.Range(0, terrainLength);
        float y = terrain.SampleHeight(new Vector3(x, 0, z));

        return new Vector3(x, y, z) + terrain.transform.position;
    }

    bool IsPositionValid(Vector3 position)
    {
        GameObject[] existingBuildings = GameObject.FindGameObjectsWithTag("Building");

        foreach(GameObject building in existingBuildings)
        {
            if(Vector3.Distance(building.transform.position, position) < minDistanceBetweenBuildings)
            {
                return false;
            }
        }

        return true;
    }
}