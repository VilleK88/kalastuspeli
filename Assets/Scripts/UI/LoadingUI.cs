using UnityEngine;
using UnityEngine.UI;

public class LoadingUI : MonoBehaviour
{
    #region Singleton

    public static LoadingUI Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    #endregion

    public Image loadingBG;
    public GameObject loadingBarObject;
    public Image loadingBar;

    private void Start()
    {
        loadingBG = GetComponent<Image>();
    }
}