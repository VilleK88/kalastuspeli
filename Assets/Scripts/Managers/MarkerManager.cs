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
    public int markerCount = 9;
    public float areaSize = 1000f;

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

        for (int i = 0; i < markerCount; i++)
        {
            Vector3 randomPoint = GetRandomPointOnNavMesh(transform.position, areaSize);
            if (randomPoint != Vector3.zero)
            {
                GameObject prefabInstance = Instantiate(markerPrefab, randomPoint, Quaternion.identity);
                Marker marker = prefabInstance.GetComponent<Marker>();

                int industryCount = System.Enum.GetValues(typeof(IndustryType)).Length;
                int randomIndex = Random.Range(0, industryCount);

                IndustryType randomIndustry = (IndustryType)System.Enum.GetValues(typeof(IndustryType)).GetValue(randomIndex);
                marker.industryType = randomIndustry;

                int randomI = Random.Range(0, cityCompanies.Count);
                marker.yritys = cityCompanies[randomI];
            }
        }
    }

    Vector3 GetRandomPointOnNavMesh(Vector3 center, float range)
    {
        for (int i = 0; i < 30; i++)
        {
            Vector3 randomPos = center + new Vector3(
                Random.Range(-range, range),
                0,
                Random.Range(-range, range));

            if (NavMesh.SamplePosition(randomPos, out NavMeshHit hit, 2f, NavMesh.AllAreas))
                return hit.position;
        }

        return Vector3.zero;
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
            Debug.Log("Raw JSON: " + json);
            YritysApiResponse vastaus = JsonUtility.FromJson<YritysApiResponse>(json);

            foreach (var yritys in vastaus.results)
            {
                if(yritys == null)
                {
                    Debug.LogWarning("Null company found!");
                    continue;
                }

                cityCompanies = vastaus.results.ToList();

                Debug.Log($"🔹 {yritys.nimi ?? "(no name)"} - {yritys.kunta ?? "(no municipality)"}");
            }
        }
    }
}