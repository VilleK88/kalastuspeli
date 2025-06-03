using UnityEngine;
using UnityEngine.AI;

public class Test : MonoBehaviour
{
    [SerializeField] GameObject markerPrefab;
    public int markerCount = 10;
    public float areaSize = 20f;

    private void Start()
    {
        for(int i = 0; i < markerCount; i++)
        {
            Vector3 randomPoint = GetRandomPointOnNavMesh(transform.position, areaSize);
            if(randomPoint != Vector3.zero)
            {
                Instantiate(markerPrefab, randomPoint, Quaternion.identity);
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
                Random.Range(-range, range)
            );

            if(NavMesh.SamplePosition(randomPos, out NavMeshHit hit, 2f, NavMesh.AllAreas))
            {
                return hit.position;
            }
        }

        return Vector3.zero;
    }
}