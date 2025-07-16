using UnityEngine;
using UnityEngine.UI;

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
}