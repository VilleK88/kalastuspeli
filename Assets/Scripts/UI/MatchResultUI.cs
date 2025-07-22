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
    public GameObject showJobsButton;
    public GameObject playAgainButton;
    public GameObject gameOverScreen;

    public void ShowResult(string resultString, GameObject medal)
    {
        transparentBG.SetActive(true);
        showJobsButton.SetActive(true);
        resultText.text = resultString;
        medal.SetActive(true);
    }

    public void ShowGameOverScreen()
    {
        transparentBG.SetActive(true);
        playAgainButton.SetActive(true);
        gameOverScreen.SetActive(true);
        resultText.text = "Hidden jobs are an opportunity, don’t hang yourself.";
    }

    public void CloseGameOverScreen()
    {
        playAgainButton.SetActive(false);
        gameOverScreen.SetActive(false);
        transparentBG.SetActive(false);
    }

    public void PlayAgain()
    {
        JobApplicationsManager.Instance.playerJobApp.jobApplicationCount = 0;
        JobApplicationsManager.Instance.playerJobApp.countText.text = "0";
        JobApplicationsManager.Instance.rivalJobApp.jobApplicationCount = 0;
        JobApplicationsManager.Instance.rivalJobApp.countText.text = "0";

        GameTimer.Instance.currentGameTimeMin = 0;
        GameTimer.Instance.currentGameTimeSec = 0;

        BurnoutMeter.Instance.Heal(100);

        CloseGameOverScreen();
        PauseManager.Instance.ResumeGame();
    }

    public void GoToThisScene()
    {
        PauseManager.Instance.ResumeGame();
        SceneManager.LoadScene("2 - Map");
        Debug.Log("Change scene to: " + "2 - Map");
    }
}