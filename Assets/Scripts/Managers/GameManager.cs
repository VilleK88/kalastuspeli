using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

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

            if(loadingUIInstance == null && loadingUIPrefab != null)
            {
                loadingUIInstance = Instantiate(loadingUIPrefab);
                DontDestroyOnLoad(loadingUIInstance);
            }
        }
        else
            Destroy(gameObject);
    }
    #endregion

    public City city;
    public PlayerCharacter character;

    [Header("Scenes to Load")]
    [SerializeField] SceneField _levelScene;

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

        yield return new WaitForSeconds(0.1f);

        AsyncOperation operation = SceneManager.LoadSceneAsync(_levelScene, LoadSceneMode.Single);
        operation.allowSceneActivation = false;

        while(operation.progress < 0.9f)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            LoadingUI.Instance.UpdateProgress(progress);
            yield return null;
        }

        yield return new WaitForSeconds(0.5f);

        operation.allowSceneActivation = true;

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (loadingUIInstance != null)
            LoadingUI.Instance.Hide();

        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}