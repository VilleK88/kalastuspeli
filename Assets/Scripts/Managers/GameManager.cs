using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject loadingUIPrefab;
    GameObject loadingUIInstance;

    #region Singleton
    public static GameManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }
        else
            Destroy(gameObject);
    }
    #endregion

    public City city;
    public PlayerCharacter character;

    [Header("Scenes to Load")]
    [SerializeField] SceneField _levelScene;

    GameObject cityPrefab;
    GameObject cityInstance;

    public void CreateLoadingCanvas()
    {
        if (loadingUIInstance == null && loadingUIPrefab != null)
        {
            loadingUIInstance = Instantiate(loadingUIPrefab);
            DontDestroyOnLoad(loadingUIInstance);
        }
    }

    public void SetCity(City cityToGo)
    {
        city = cityToGo;
    }

    public void SetCharacter(PlayerCharacter characterToGo)
    {
        character = characterToGo;
    }

    public void LoadCityScene()
    {
        StartCoroutine(LoadCitySceneCoroutine());
    }

    IEnumerator LoadCitySceneCoroutine()
    {
        LoadingUI.Instance.Show();

        yield return new WaitForSeconds(0.3f);

        GetCityPrefabReady();

        AsyncOperation operation = SceneManager.LoadSceneAsync(_levelScene, LoadSceneMode.Single);
        operation.allowSceneActivation = false;

        while(operation.progress < 0.9f)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            LoadingUI.Instance.UpdateProgress(progress);
            yield return null;
        }

        LoadingUI.Instance.UpdateProgress(1f);

        SceneManager.sceneLoaded += OnSceneLoaded;
        yield return new WaitForSeconds(0.5f);

        operation.allowSceneActivation = true;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (cityInstance != null)
            cityInstance.SetActive(true);

        //if (loadingUIInstance != null)
            //LoadingUI.Instance.Hide();

        SceneManager.sceneLoaded -= OnSceneLoaded;

        if (loadingUIInstance != null)
            LoadingUI.Instance.Hide();
    }

    public void DestroyCityInstance()
    {
        if(cityInstance != null)
            Destroy(cityInstance);
    }

    void GetCityPrefabReady()
    {
        string cityName = city.ToString();
        string path = "Cities/" + cityName;

        cityPrefab = Resources.Load<GameObject>(path);

        if(cityPrefab != null)
        {
            cityInstance = Instantiate(cityPrefab);
            cityInstance.SetActive(false);
            DontDestroyOnLoad(cityInstance);
        }
        else
            Debug.LogWarning($"City prefab not found at Resources/{path}");
    }
}