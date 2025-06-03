using System.Collections;
using UnityEngine;
using UnityEngine.AI;

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

    public void InitializeMarkers()
    {
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
            }
        }
    }

    Vector3 GetRandomPointOnNavMesh(Vector3 center, float range)
    {
        for(int i = 0; i < 30; i++)
        {
            Vector3 randomPos = center + new Vector3(
                Random.Range(-range, range),
                0,
                Random.Range(-range, range));

            if(NavMesh.SamplePosition(randomPos, out NavMeshHit hit, 2f, NavMesh.AllAreas))
                return hit.position;
        }

        return Vector3.zero;
    }
}