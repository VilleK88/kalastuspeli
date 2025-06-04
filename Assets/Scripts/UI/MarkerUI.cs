using UnityEngine;
using TMPro;
using System.Collections;
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
    [SerializeField] TMP_InputField inputField;
    public bool open;

    public void OpenMarkerInfoPanel()
    {
        open = true;
        StartCoroutine(DelayedInfoPanelOpening(4f));
    }

    public void CloseMarkerInfoPanel()
    {
        transparentBG.SetActive(false);
        open = false;
        MouseManager.Instance.StopFishing();
    }

    IEnumerator DelayedInfoPanelOpening(float time)
    {
        yield return new WaitForSeconds(time);
        transparentBG.SetActive(true);
    }

    public void OpenGoogleMaps()
    {
        if(inputField != null)
            OpenAddressInGoogleMaps(inputField.text);
        else
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