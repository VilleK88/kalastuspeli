using UnityEngine;

public class MapManager : MonoBehaviour
{
    [SerializeField] GameObject[] mapPrefabs;

    private void Start()
    {
        if(GameManager.Instance != null)
        {
            City city = GameManager.Instance.city;
            string cityName = city.ToString();
            
            for(int i = 0; i < mapPrefabs.Length; i++)
            {
                if (mapPrefabs[i].name.Contains(cityName))
                {
                    mapPrefabs[i].SetActive(true);
                }
            }
        }
    }
}