using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Networking;
using System.Linq;

public class MarkerManager : MonoBehaviour
{
    #region Singleton
    public static MarkerManager Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    #endregion

    [SerializeField] GameObject markerPrefab;
    public int markerCount = 30;
    public float areaSize = 1000f;
    public int currentCount = 0;

    [Header("Company Parameters")]
    public string apiURL;
    List<Yritys> cityCompanies = new List<Yritys>();

    public void InitializeMarkers()
    {
        StartCoroutine(FetchCompanies());
        StartCoroutine(DelayedMarkerGeneration(1f));
    }

    IEnumerator DelayedMarkerGeneration(float time)
    {
        yield return new WaitForSeconds(time);
        GenerateMarkers();
    }

    void GenerateMarkers()
    {
        List<GameObject> gridObjectList = GetGridList(currentCount);

        for (int i = 0; i < markerCount; i++)
        {
            if(gridObjectList.Count > 0)
            {
                int randomIndex = Random.Range(0, gridObjectList.Count);
                GameObject gridObject = gridObjectList[randomIndex];
                GridPrefab gridPrefab = gridObject.GetComponent<GridPrefab>();
                gridPrefab.markerCount += 1;
                Vector3 randomPoint = GetRandomPointInGridCell(gridObject, 1.25f);
                CreateMarker(randomPoint, gridObject.transform);
                gridObjectList.RemoveAt(randomIndex);
                Debug.Log("Gridobject count: " + gridObjectList.Count);
            }
            else
            {
                Debug.Log("Get new list");
                currentCount += 1;
                gridObjectList = GetGridList(currentCount);
            }

        }
    }

    List<GameObject> GetGridList(int count)
    {
        List<GameObject> gridObjectList = new List<GameObject>();
        for(int i = 0; i < GridManager.Instance.grid.Count; i++)
        {
            GameObject gridObject = GridManager.Instance.grid[i];
            GridPrefab gridPrefab = gridObject.GetComponent<GridPrefab>();
            if(gridPrefab.markerCount == count)
            {
                gridObjectList.Add(gridObject);
            }
        }

        return gridObjectList;
    }

    void CreateMarker(Vector3 randomPoint, Transform parentObject)
    {
        GameObject prefabInstance = Instantiate(markerPrefab, randomPoint, Quaternion.identity);
        //prefabInstance.transform.parent = Instance.transform;
        prefabInstance.transform.parent = parentObject;
        Marker marker = prefabInstance.GetComponent<Marker>();

        int industryCount = System.Enum.GetValues(typeof(IndustryType)).Length;
        int randomIndex = Random.Range(0, industryCount);

        IndustryType randomIndustry = (IndustryType)System.Enum.GetValues(typeof(IndustryType)).GetValue(randomIndex);
        marker.industryType = randomIndustry;

        if (cityCompanies != null)
        {
            if (cityCompanies.Count > 0)
            {
                int randomI = Random.Range(0, cityCompanies.Count);
                marker.yritys = cityCompanies[randomI];
            }
        }
    }

    Vector3 GetRandomChildCell(GameObject gridObject)
    {
        List<GameObject> cellsInGrid = new List<GameObject>();
        for(int i = 0; i < gridObject.transform.childCount; i++)
        {
            GameObject cell = gridObject.transform.GetChild(i).gameObject;
            if (cell.transform.childCount == 0)
                cellsInGrid.Add(cell);
        }

        int randomIndex = Random.Range(0, cellsInGrid.Count);

        return cellsInGrid[randomIndex].transform.position;
    }

    Vector3 GetRandomPointInGridCell(GameObject gridObject, float cellSize)
    {
        Vector3 gridCenter = GetRandomChildCell(gridObject);

        float half = cellSize / 2;

        for(int i = 0; i < 30; i++)
        {
            float offsetX = Random.Range(-half, half);
            float offsetZ = Random.Range(-half, half);

            Vector3 randomPosition = new Vector3(gridCenter.x + offsetX, gridCenter.y, gridCenter.z + offsetZ);

            if (NavMesh.SamplePosition(randomPosition, out NavMeshHit hit, 1, NavMesh.AllAreas))
                return hit.position;
        }

        return gridCenter;
    }

    IEnumerator FetchCompanies()
    {
        string currentCity = GameManager.Instance.city.ToString();
        string URL = apiURL + "?kunta=" + currentCity.ToUpper();
        UnityWebRequest request = UnityWebRequest.Get(URL);
        yield return request.SendWebRequest();

#if UNITY_2020_1_OR_NEWER
        if (request.result != UnityWebRequest.Result.Success)
#else
        if (request.isNetworkError || request.isHttpError)
#endif
        {
            Debug.LogError("API-virhe: " + request.error);
        }
        else
        {
            string json = request.downloadHandler.text;
            //Debug.Log("Raw JSON: " + json);
            YritysApiResponse vastaus = JsonUtility.FromJson<YritysApiResponse>(json);

            foreach (var yritys in vastaus.results)
            {
                if(yritys == null)
                {
                    Debug.LogWarning("Null company found!");
                    continue;
                }

                cityCompanies = vastaus.results.ToList();

                //Debug.Log($"🔹 {yritys.nimi ?? "(no name)"} - {yritys.kunta ?? "(no municipality)"}");
            }
        }
    }
}