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
    public GameObject innerBG;

    public GameObject innerBG_JobApplicationsList;
    public GameObject jobListingContent;
    public GameObject jobListingButtonObj;

    public void ShowResult(string resultString, GameObject medal)
    {
        MouseManager.Instance.StopWalking();
        MouseManager.Instance.playerAnim.SetBool("Fishing_Idle", false);
        MouseManager.Instance.playerAnim.Play("Idle");

        MarkerUI.Instance.CloseMarkerInfoPanel();
        transparentBG.SetActive(true);
        innerBG.SetActive(true);
        showJobsButton.SetActive(true);
        resultText.enabled = true;
        resultText.text = resultString;
        medal.SetActive(true);
    }

    public void ShowGameOverScreen()
    {
        MouseManager.Instance.StopWalking();
        MouseManager.Instance.playerAnim.SetBool("Fishing_Idle", false);
        MouseManager.Instance.playerAnim.Play("Idle");

        transparentBG.SetActive(true);
        innerBG.SetActive(true);
        playAgainButton.SetActive(true);
        gameOverScreen.SetActive(true);
        resultText.enabled = true;
        resultText.text = "Hidden jobs are an opportunity, don’t hang yourself.";
    }

    public void CloseGameOverScreen()
    {
        playAgainButton.SetActive(false);
        innerBG_JobApplicationsList.SetActive(false);
        gameOverScreen.SetActive(false);
        transparentBG.SetActive(false);
        MarkerManager.Instance.collectedJobListings.Clear();
    }

    public void PlayAgain()
    {
        AudioManager.Instance.PlaySFX("ButtonClick");
        JobApplicationsManager.Instance.playerJobApp.jobApplicationCount = 0;
        JobApplicationsManager.Instance.playerJobApp.countText.text = "0";
        JobApplicationsManager.Instance.rivalJobApp.jobApplicationCount = 0;
        JobApplicationsManager.Instance.rivalJobApp.countText.text = "0";

        GameTimer.Instance.currentGameTimeMin = 0;
        GameTimer.Instance.currentGameTimeSec = 0;

        BurnoutMeter.Instance.Heal(100);

        CloseGameOverScreen();
        PauseManager.Instance.ResumeGame();
        MouseManager.Instance.ReturnToStartPosition();

        RivalJobApplicant rival = FindAnyObjectByType<RivalJobApplicant>();
        rival.ReturnToStartPosition();
    }

    public void GoToThisScene()
    {
        AudioManager.Instance.PlaySFX("ButtonClick");
        PauseManager.Instance.ResumeGame();
        SceneManager.LoadScene("2 - Map");
        Debug.Log("Change scene to: " + "2 - Map");
    }

    public void ShowJobs()
    {
        AudioManager.Instance.PlaySFX("ButtonClick");
        innerBG.SetActive(false);
        innerBG_JobApplicationsList.SetActive(true);

        for(int i = 0; i < MarkerManager.Instance.collectedJobListings.Count; i++)
        {
            GameObject prefabInstance = Instantiate(jobListingButtonObj, transform.position, Quaternion.identity);
            prefabInstance.transform.parent = jobListingContent.transform;
            JobListingButton jobListingButton = prefabInstance.GetComponent<JobListingButton>();
            jobListingButton.jobListing = MarkerManager.Instance.collectedJobListings[i];
            jobListingButton.text.text = jobListingButton.jobListing.title.ToString();
        }

        showJobsButton.SetActive(false);
        playAgainButton.SetActive(true);
    }
}