using UnityEngine;

public class MenuUIManager : MonoBehaviour
{
    [SerializeField] GameObject eventPanelUserInRange;
    [SerializeField] GameObject eventPanelUserNotInRange;
    bool isUIPanelActive;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void DisplayStartEventPanel()
    {
        if(!isUIPanelActive)
        {
            eventPanelUserInRange.SetActive(true);
            isUIPanelActive = true;
        }
    }

    public void DisplayUserNotInRangePanel()
    {
        if(!isUIPanelActive)
        {
            eventPanelUserNotInRange.SetActive(true);
            isUIPanelActive = true;
        }
    }

    public void CloseButtonClick()
    {
        eventPanelUserInRange.SetActive(false);
        eventPanelUserNotInRange.SetActive(false);
        isUIPanelActive = false;
    }
}