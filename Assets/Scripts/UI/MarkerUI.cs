using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using System.Reflection;
using System.Globalization;

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
    [SerializeField] Yritys yritys;
    [SerializeField] GameObject companyInfoObject;
    [SerializeField] TextMeshProUGUI companyName;
    [SerializeField] TextMeshProUGUI businessID;
    [SerializeField] TextMeshProUGUI founded;
    [SerializeField] TextMeshProUGUI postalAddress;
    [SerializeField] TextMeshProUGUI postcode;
    [SerializeField] TextMeshProUGUI municipality;

    [Header("Job Listing Parameters")]
    [SerializeField] JobListing jobListing;
    [SerializeField] GameObject jobInfoObject;
    [SerializeField] Transform contentTransform;
    [SerializeField] TextMeshProUGUI title;
    [SerializeField] TextMeshProUGUI link;
    [SerializeField] TextMeshProUGUI company;
    [SerializeField] TextMeshProUGUI location;
    [SerializeField] TextMeshProUGUI published;
    [SerializeField] TextMeshProUGUI deadline;
    [SerializeField] TextMeshProUGUI summary;
    [SerializeField] TextMeshProUGUI[] job_types;
    [SerializeField] TextMeshProUGUI salary_basis;
    [SerializeField] TextMeshProUGUI salary;
    [SerializeField] TextMeshProUGUI language;
    [SerializeField] TextMeshProUGUI employment_type;
    [SerializeField] TextMeshProUGUI working_hours;
    [SerializeField] TextMeshProUGUI start_date;
    [SerializeField] TextMeshProUGUI business_id;
    [SerializeField] TextMeshProUGUI company_description;
    [SerializeField] TextMeshProUGUI open_positions;
    [SerializeField] TextMeshProUGUI application_link;
    [SerializeField] TextMeshProUGUI locations;
    [SerializeField] TextMeshProUGUI address;

    [SerializeField] TextMeshProUGUI textPrefab;

    public void UpdateCompanyParameters(Yritys currentCompany)
    {
        yritys = currentCompany;

        /*companyName.text = currentCompany.nimi ?? "-";
        businessID.text = currentCompany.y_tunnus ?? "-";
        founded.text = currentCompany.perustettu ?? "-";
        postalAddress.text = currentCompany.postiosoite_katu ?? "-";
        postcode.text = currentCompany.postinumero ?? "-";
        municipality.text = currentCompany.kunta ?? "-";*/
    }

    public void UpdateJobListingParameters(JobListing currentJob)
    {
        jobListing = currentJob;
        OpenJobListingInfo();
    }

    public void OpenMarkerInfoPanel()
    {
        open = true;
        StartCoroutine(DelayedInfoPanelOpening(4f));
    }

    public void CloseMarkerInfoPanel()
    {
        companyInfoObject.SetActive(false);
        jobInfoObject.SetActive(false);
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
        //companyInfoObject.SetActive(true);
        jobInfoObject.SetActive(true);
    }

    void OpenJobListingInfo()
    {
        foreach(Transform child in contentTransform)
        {
            Destroy(child.gameObject);
        }

        FieldInfo[] fields = typeof(JobListing).GetFields();

        foreach(FieldInfo field in fields)
        {
            object value = field.GetValue(jobListing);

            string fieldName = field.Name;
            string fieldValue;

            if (value == null)
                fieldValue = "-";
            else if (field.FieldType == typeof(string[]))
                fieldValue = string.Join(", ", (string[])value);
            else if (field.FieldType == typeof(Contact[]))
                fieldValue = $"({((Contact[])value).Length} contact(s))";
            else
                fieldValue = value.ToString();

            TextMeshProUGUI tmp = Instantiate(textPrefab, contentTransform);
            tmp.text = $"<b>{fieldName}:</b> {fieldValue}";
        }
    }

    public void OpenGoogleMaps()
    {
        if (jobListing.address != null)
            OpenAddressInGoogleMaps(jobListing.address);
        else
            OpenAddressInGoogleMaps("Karamalmin kampus");
    }

    public void OpenAddressInGoogleMaps(string address)
    {
        Application.OpenURL("https://www.google.com/maps/search/?api=1&query=" + UnityWebRequest.EscapeURL(address));
    }
}