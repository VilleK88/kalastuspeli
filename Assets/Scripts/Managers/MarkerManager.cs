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
    [HideInInspector] public string jobListing_ApiURL;
    [HideInInspector] public string companyInfo_ApiURL;
    [SerializeField] ApiConfig jobListingApiConfig;
    [SerializeField] ApiConfig companyInfoApiConfig;
    List<Yritys> cityCompanies = new List<Yritys>();
    public List<JobListing> cityJobListings = new List<JobListing>();
    public List<JobListing> collectedJobListings = new List<JobListing>();

    [Header("Marker Parameters")]
    public Marker currentMarker;
    public GridPrefab currentGridPrefab; // this variable is used when marker is destroyed to make sure new marker isn't generated on the same gridprefab.

    public void InitializeMarkers()
    {
        if(jobListingApiConfig != null)
            jobListing_ApiURL = jobListingApiConfig.apiURL.ToString();

        if (companyInfoApiConfig != null)
            companyInfo_ApiURL = companyInfoApiConfig.apiURL.ToString();

        StartCoroutine(InitializeMarkerData());
    }

    IEnumerator InitializeMarkerData()
    {
        yield return StartCoroutine(FetchJobs());
        yield return StartCoroutine(FetchCompanies());
        yield return new WaitForSeconds(1);
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
                Transform gridCellTransform = GetRandomGridCellPrefab(gridObject);
                Vector3 randomPoint = GetRandomPointInGridPrefabsCell(gridCellTransform, 1.25f);
                CreateMarker(randomPoint, gridCellTransform);
                gridObjectList.RemoveAt(randomIndex);
            }
            else
            {
                currentCount += 1;
                gridObjectList = GetGridList(currentCount);
            }

        }
    }

    public void GenerateNewMarker()
    {
        List<GameObject> gridObjectList = GetGridList(1);

        if (gridObjectList.Count == 0)
            gridObjectList = GetGridList(2);

        if(currentGridPrefab != null)
        {
            gridObjectList.Remove(currentGridPrefab.gameObject);
            currentGridPrefab = null;
        }

        while (true)
        {
            int randomIndex = Random.Range(0, gridObjectList.Count);
            GridPrefab gridPrefab = gridObjectList[randomIndex].GetComponent<GridPrefab>();
            if(gridPrefab.markerCount <= 1)
            {
                Transform gridCellTransform = GetRandomGridCellPrefab(gridPrefab.gameObject);
                Vector3 randomPoint = GetRandomPointInGridPrefabsCell(gridCellTransform, 1.25f);
                CreateMarker(randomPoint, gridPrefab.gameObject.transform);
                gridPrefab.markerCount++;
                break;
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
            if(gridPrefab.markerCount <= count)
            {
                gridObjectList.Add(gridObject);
            }
        }

        return gridObjectList;
    }

    void CreateMarker(Vector3 randomPoint, Transform parentObject)
    {
        GameObject prefabInstance = Instantiate(markerPrefab, randomPoint, Quaternion.identity);
        prefabInstance.transform.parent = parentObject;
        Marker marker = prefabInstance.GetComponent<Marker>();

        int industryCount = System.Enum.GetValues(typeof(IndustryType)).Length;
        int randomIndex = Random.Range(0, industryCount);

        IndustryType randomIndustry = (IndustryType)System.Enum.GetValues(typeof(IndustryType)).GetValue(randomIndex);
        marker.industryType = randomIndustry;

        if (cityCompanies.Count > 0)
        {
            int randomI = Random.Range(0, cityCompanies.Count);
            marker.yritys = cityCompanies[randomI];
        }

        if (cityJobListings != null)
        {
            if(cityJobListings.Count > 0)
            {
                int randomI = Random.Range(0, cityJobListings.Count);
                marker.jobListing = cityJobListings[randomI];
            }
        }
    }

    Transform GetRandomGridCellPrefab(GameObject gridObject)
    {
        List<GameObject> cellsInGrid = new List<GameObject>();
        for(int i = 0; i < gridObject.transform.childCount; i++)
        {
            GameObject cell = gridObject.transform.GetChild(i).gameObject;
            if (cell.transform.childCount == 0)
                cellsInGrid.Add(cell);
        }

        int randomIndex = Random.Range(0, cellsInGrid.Count);

        return cellsInGrid[randomIndex].transform;
    }

    Vector3 GetRandomPointInGridPrefabsCell(Transform gridCellTransform, float cellSize)
    {
        Vector3 gridCellCenter = gridCellTransform.position;

        float half = cellSize / 2;

        for(int i = 0; i < 30; i++)
        {
            float offsetX = Random.Range(-half, half);
            float offsetZ = Random.Range(-half, half);

            Vector3 randomPosition = new Vector3(gridCellCenter.x + offsetX, gridCellCenter.y, gridCellCenter.z + offsetZ);

            if(Physics.Raycast(randomPosition, Vector3.down, out RaycastHit rayHit, 100, ~LayerMask.GetMask("Obstacle")))
            {
                Vector3 candidate = rayHit.point;

                if (NavMesh.SamplePosition(candidate, out NavMeshHit hit, 1, NavMesh.AllAreas))
                    return hit.position;
            }
        }

        return gridCellCenter;
    }

    IEnumerator FetchJobs()
    {
        string currentCity = GameManager.Instance.city.ToString();
        string URL = jobListing_ApiURL + "?location=" + currentCity.ToUpper();
        Debug.Log("URL: " + URL);
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
            JobListingResponse response = JsonUtility.FromJson<JobListingResponse>(json);

            foreach (var job in response.results)
            {
                if(job == null)
                {
                    Debug.LogWarning("Null job found!");
                    continue;
                }
            }
            cityJobListings = response.results.ToList();
        }
    }

    IEnumerator FetchCompanies()
    {
        string currentCity = GameManager.Instance.city.ToString();
        string URL = companyInfo_ApiURL + "?kunta=" + currentCity.ToUpper();
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
            YritysApiResponse vastaus = JsonUtility.FromJson<YritysApiResponse>(json);

            foreach (var yritys in vastaus.results)
            {
                if (yritys == null)
                {
                    Debug.LogWarning("Null company found!");
                    continue;
                }

                cityCompanies = vastaus.results.ToList();
            }
        }
    }
}