using UnityEngine;
using TMPro;

public class JobApplicationPrefab : MonoBehaviour
{
    public TextMeshProUGUI countText;
    public int jobApplicationCount = 0;

    public void IncreaseJobAppCount()
    {
        jobApplicationCount++;
        countText.text = jobApplicationCount.ToString();
    }
}