using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class MatchResultUI : MonoBehaviour
{
    #region Singleton
    public static MatchResultUI Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    #endregion

    [SerializeField] GameObject transparentBG;
    [SerializeField] TextMeshProUGUI resultText;
    public GameObject firstPlace;
    public GameObject secondPlace;

    public void ShowResult(string resultString, GameObject medal)
    {
        transparentBG.SetActive(true);
        resultText.text = resultString;
        medal.SetActive(true);
    }
}