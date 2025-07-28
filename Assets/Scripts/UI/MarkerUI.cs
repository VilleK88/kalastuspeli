using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.EventSystems;
using System.Reflection;

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
    [SerializeField] GameObject descriptionPopup;
    [SerializeField] Transform descriptionPopupTransform;

    [SerializeField] TextMeshProUGUI textPrefab;

    public void UpdateCompanyParameters(Yritys currentCompany)
    {
        yritys = currentCompany;
    }

    public void UpdateJobListingParameters(JobListing currentJob)
    {
        jobListing = currentJob;
        OpenJobListingInfo();
    }

    public void OpenMarkerInfoPanel()
    {
        if(GameTimer.Instance.currentGameTimeMin < 3)
        {
            open = true;
            StartCoroutine(DelayedInfoPanelOpening(4f));
        }
        else
        {
            transparentBG.SetActive(true);
            jobInfoObject.SetActive(true);
        }
    }

    public void CloseMarkerInfoPanel()
    {
        descriptionPopup.SetActive(false);
        companyInfoObject.SetActive(false);
        jobInfoObject.SetActive(false);
        transparentBG.SetActive(false);

        if (GameTimer.Instance.currentGameTimeMin < 3)
        {
            MouseManager.Instance.StopFishing();
            Destroy(MarkerManager.Instance.currentMarker.gameObject);
            MarkerManager.Instance.GenerateNewMarker();
            StartCoroutine(DelayedBooleanValueChange(1f));
            PauseManager.Instance.ResumeGame();
        }
    }

    IEnumerator DelayedBooleanValueChange(float time)
    {
        yield return new WaitForSeconds(time);
        open = false;
    }

    IEnumerator DelayedInfoPanelOpening(float time)
    {
        yield return new WaitForSeconds(time);
        Marker currentMarker = MarkerManager.Instance.currentMarker;
        if (currentMarker != null)
        {
            currentMarker.markerOpen = true;
            currentMarker.DecreaseGridPrefabMarkerCount();
            transparentBG.SetActive(true);
            jobInfoObject.SetActive(true);
            JobApplicationsManager.Instance.IncreasePlayersJobAppCount();
            PauseManager.Instance.PauseGame();
            BurnoutMeter.Instance.Heal(10);
        }
        else
            MouseManager.Instance.StopFishing();
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

            if(fieldName != "description")
            {
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
            else
            {
                if (value == null)
                    fieldValue = "-";
                else
                    fieldValue = value.ToString();

                TextMeshProUGUI tmp = Instantiate(textPrefab, contentTransform);
                tmp.text = $"<link=\"description\"><color=#00BFFF><u>Show Description</u></color></link>";

                TMP_LinkHandler handler = tmp.gameObject.AddComponent<TMP_LinkHandler>();
                handler.OnLinkClicked = (string linkID) =>
                {
                    foreach (Transform child in descriptionPopupTransform)
                    {
                        Destroy(child.gameObject);
                    }

                    string formattedDescription = System.Text.RegularExpressions.Regex
                    .Replace(fieldValue, @"(?<=[.!?])\s+", "\n");

                    TextMeshProUGUI tmpDescription = Instantiate(textPrefab, descriptionPopupTransform);
                    tmpDescription.text = $"<b>{fieldName}:</b>\n{formattedDescription}";

                    OpenDescriptionPopup();
                };
            }
        }
    }

    public void OpenDescriptionPopup()
    {
        jobInfoObject.SetActive(false);
        descriptionPopup.SetActive(true);
    }

    public void CloseDescriptionPopup()
    {
        descriptionPopup.SetActive(false);
        jobInfoObject.SetActive(true);
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