using UnityEngine;
using TMPro;
using System.Collections;
using System.Runtime.InteropServices;
using Unity.VisualScripting;

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

    [Header("Company Parameters")]
    public Yritys yritys;
    public TextMeshProUGUI companyName;
    public TextMeshProUGUI businessID;
    public TextMeshProUGUI founded;
    public TextMeshProUGUI postalAddress;
    public TextMeshProUGUI postcode;
    public TextMeshProUGUI municipality;

    public void UpdateCompanyParameters(Yritys currentCompany)
    {
        yritys = currentCompany;
        companyName.text = currentCompany.nimi ?? "-";
        businessID.text = currentCompany.y_tunnus ?? "-";
        founded.text = currentCompany.perustettu ?? "-";
        postalAddress.text = currentCompany.postiosoite_katu ?? "-";
        postcode.text = currentCompany.postinumero ?? "-";
        municipality.text = currentCompany.kunta ?? "-";
    }

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
        if(yritys.postiosoite_katu != null)
            OpenAddressInGoogleMaps(yritys.postiosoite_katu);
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