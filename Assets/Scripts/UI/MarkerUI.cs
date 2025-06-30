using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

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
        MouseManager.Instance.StopFishing();
        StartCoroutine(DelayedBooleanValueChange(1f));
    }

    IEnumerator DelayedBooleanValueChange(float time)
    {
        yield return new WaitForSeconds(time);
        open = false;
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

    public void OpenAddressInGoogleMaps(string address)
    {
        Application.OpenURL("https://www.google.com/maps/search/?api=1&query=" + UnityWebRequest.EscapeURL(address));
    }
}