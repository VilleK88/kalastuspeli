using UnityEngine;
using TMPro;

public class GameTimer : MonoBehaviour
{
    #region Singleton
    public static GameTimer Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    #endregion

    [SerializeField] TextMeshProUGUI timerText;
    float maxTime = 1;
    float counter = 0;
    public float currentGameTimeMin = 0;
    public float currentGameTimeSec = 0;

    private void Update()
    {
        if(counter <= maxTime)
            counter += Time.deltaTime;
        else
        {
            if(currentGameTimeSec < 59)
                currentGameTimeSec += 1;
            else
            {
                currentGameTimeMin += 1;
                currentGameTimeSec = 0;
            }

            string minutes = ReturnString(currentGameTimeMin);
            string seconds = ReturnString(currentGameTimeSec);
            timerText.text = minutes + ":" + seconds;

            counter = 0;

            if(currentGameTimeMin >= 3)
            {
                JobApplicationsManager.Instance.ShowEndingScreen();
                PauseManager.Instance.PauseGame();
            }
        }
    }

    string ReturnString(float time)
    {
        string timeText;
        if (time < 10)
            timeText = "0" + time.ToString();
        else
            timeText = time.ToString();

        return timeText;
    }
}