using UnityEngine;

public class MapDeviceCheck : MonoBehaviour
{
    public bool isMobile;
    [SerializeField] GameObject mapOfFinlandHighRes;
    [SerializeField] GameObject mapOfFinlandLowRes;

    private void Awake()
    {
        isMobile = Application.isMobilePlatform;
        Debug.Log("Mobile device: " + isMobile);
    }

    private void Start()
    {
        if (!isMobile)
            mapOfFinlandHighRes.SetActive(true);
        else
            mapOfFinlandLowRes.SetActive(true);
    }
}