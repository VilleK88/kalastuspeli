using UnityEngine;
using System.Runtime.InteropServices;

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

    public void OpenGoogleMaps()
    {
        OpenAddressInGoogleMaps("Karamalmin kampus");
    }

#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
        private static extern void OpenGoogleMaps(string address);
#endif

    public void OpenAddressInGoogleMaps(string address)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        OpenGoogleMaps(address);
#else
        Debug.Log("Google Maps opening is only supported in WebGL builds.");
#endif
    }
}