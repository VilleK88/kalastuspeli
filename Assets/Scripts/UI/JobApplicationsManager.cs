using UnityEngine;

public class JobApplicationsManager : MonoBehaviour
{
    #region Singleton
    public static JobApplicationsManager Instance;
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    #endregion

    public JobApplicationPrefab playerJobApp;
    public JobApplicationPrefab rivalJobApp;

    public void IncreasePlayersJobAppCount()
    {
        playerJobApp.IncreaseJobAppCount();
    }

    public void IncreaseRivalsJobAppCount()
    {
        rivalJobApp.IncreaseJobAppCount();
    }

    public void ShowEndingScreen()
    {
        if (playerJobApp.jobApplicationCount > rivalJobApp.jobApplicationCount)
            MatchResultUI.Instance.ShowResult("1st place", MatchResultUI.Instance.firstPlace, "1stPlace");
        else if (playerJobApp.jobApplicationCount < rivalJobApp.jobApplicationCount)
            MatchResultUI.Instance.ShowResult("2nd place", MatchResultUI.Instance.secondPlace, "2ndPlace");
        else if (playerJobApp.jobApplicationCount == rivalJobApp.jobApplicationCount)
            MatchResultUI.Instance.ShowResult("Draw", MatchResultUI.Instance.secondPlace, "Failure");
    }
}