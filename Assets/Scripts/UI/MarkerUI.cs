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
    [SerializeField] GameObject companyInfoObject;
    [SerializeField] TextMeshProUGUI companyName;
    [SerializeField] TextMeshProUGUI businessID;
    [SerializeField] TextMeshProUGUI founded;
    [SerializeField] TextMeshProUGUI postalAddress;
    [SerializeField] TextMeshProUGUI postcode;
    [SerializeField] TextMeshProUGUI municipality;

    [Header("Job Listing Parameters")]
    [SerializeField] GameObject jobInfoObject;
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

    public void UpdateCompanyParameters(Yritys currentCompany)
    {
        companyName.text = currentCompany.nimi ?? "-";
        businessID.text = currentCompany.y_tunnus ?? "-";
        founded.text = currentCompany.perustettu ?? "-";
        postalAddress.text = currentCompany.postiosoite_katu ?? "-";
        postcode.text = currentCompany.postinumero ?? "-";
        municipality.text = currentCompany.kunta ?? "-";
    }

    public void UpdateJobListingParameters(JobListing currentJob)
    {
        title.text = currentJob.title ?? "-";
        link.text = currentJob.link ?? "-";
        company.text = currentJob.company ?? "-";
        location.text = currentJob.location ?? "-";
        published.text = currentJob.published ?? "-";
        deadline.text = currentJob.deadline ?? "-";
        summary.text = currentJob.summary ?? "-";

        job_types = null;
        for(int i = 0; i < currentJob.job_types.Length; i++)
        {
            job_types[i].text = currentJob.job_types[i];
        }

        /*salary_basis.text = currentJob.salary_basis ?? "-";
        salary.text = currentJob.salary ?? "-";
        language.text = currentJob.language ?? "-";
        employment_type.text = currentJob.employement_type ?? "-";
        working_hours.text = currentJob.working_hours ?? "-";
        start_date.text = currentJob.start_date ?? "-";
        business_id.text = currentJob.business_id ?? "-";
        company_description.text = currentJob.company_description ?? "-";
        open_positions.text = currentJob.open_positions.ToString();
        application_link.text = currentJob.application_link ?? "-";
        locations.text = currentJob.locations ?? "-";
        address.text = currentJob.address ?? "-";*/
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

    public void OpenGoogleMaps()
    {
        if (postalAddress != null)
            OpenAddressInGoogleMaps(postalAddress.text);
        else
            OpenAddressInGoogleMaps("Karamalmin kampus");
    }

    public void OpenAddressInGoogleMaps(string address)
    {
        Application.OpenURL("https://www.google.com/maps/search/?api=1&query=" + UnityWebRequest.EscapeURL(address));
    }
}