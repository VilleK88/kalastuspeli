using UnityEngine;
using TMPro;

public class JobApplicationPrefab : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI countText;
    public int jobApplicationCount = 0;

    public void IncreaseJobAppCount()
    {
        jobApplicationCount++;
        countText.text = jobApplicationCount.ToString();
    }
}