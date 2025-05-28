using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;

public class GameManager : MonoBehaviour
{
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
    [SerializeField] SceneField _persistentGameplay;
    [SerializeField] SceneField _levelScene;

    List<AsyncOperation> _scenesToLoad = new List<AsyncOperation>();

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
        //LoadingUI.Instance.loadingBG.enabled = true;
        //LoadingUI.Instance.loadingBarObject.SetActive(true);

        //_scenesToLoad.Add(SceneManager.LoadSceneAsync(_levelScene));
        //StartCoroutine(ProgressLoadingBar());
        StartCoroutine(LoadCitySceneCoroutine());
    }

    IEnumerator LoadCitySceneCoroutine()
    {
        LoadingUI.Instance.loadingBG.enabled = true;
        LoadingUI.Instance.loadingBarObject.SetActive(true);
        yield return null;
        //_scenesToLoad.Add(SceneManager.LoadSceneAsync(_levelScene));
        var operation = SceneManager.LoadSceneAsync(_levelScene, LoadSceneMode.Single);
        StartCoroutine(ProgressLoadingBar(operation));
    }

    IEnumerator ProgressLoadingBar(AsyncOperation operation)
    {
        while(!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            LoadingUI.Instance.loadingBar.fillAmount = progress;
            yield return null;
        }
    }
}