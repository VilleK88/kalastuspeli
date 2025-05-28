using UnityEngine;

public class MarkerUI : MonoBehaviour
{
    #region Singleton
    public static MarkerUI Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    #endregion

    [SerializeField] GameObject transparentBG;
    public bool open;

    public void OpenMarkerInfoPanel()
    {
        transparentBG.SetActive(true);
        open = true;
    }

    public void CloseMarkerInfoPanel()
    {
        transparentBG.SetActive(false);
        open = false;
    }
}