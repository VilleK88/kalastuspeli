using System.Collections.Generic;
using UnityEngine;

public class Marker : MonoBehaviour
{
    [SerializeField] GameObject circleObject;
    [SerializeField] ParticleSystem particleSystem;
    public bool canInteract;
    public IndustryType industryType;
    public IndustryData industryData;

    [Header("Company Parameters")]
    public Yritys yritys;
    [Header("JobListing Parameters")]
    public JobListing jobListing;

    public bool markerOpen;

    private void Start()
    {
        InitializeMarker();
    }

    public void InitializeMarker()
    {
        IndustryData[] allIndustryData = Resources.LoadAll<IndustryData>("Industries");

        foreach (var data in allIndustryData)
        {
            if (data.industryType == industryType)
            {
                industryData = data;
                break;
            }
        }

        if (industryData == null)
        {
            Debug.LogError($"IndustryData not found for type {industryType} on {gameObject.name}");
            return;
        }

        if (industryData.prefabIcon != null)
        {
            GameObject iconInstance = Instantiate(industryData.prefabIcon, transform);
            iconInstance.transform.localPosition = Vector3.zero;
            int markerLayer = LayerMask.NameToLayer("Marker");
            iconInstance.layer = markerLayer;
        }
        else
            Debug.LogWarning($"Prefab icon missing for {industryType} on {gameObject.name}");
    }

    public void EnableSFX()
    {
        canInteract = true;
        particleSystem.Play();
    }

    public void DisableSFX()
    {
        canInteract = false;
        particleSystem.Stop();
    }

    public void StartInteraction()
    {
        if(canInteract && !MouseManager.Instance.walking)
        {
            MouseManager.Instance.LookAtMarker(transform);
            if(yritys != null)
                MarkerUI.Instance.UpdateCompanyParameters(yritys);
            if (jobListing != null)
                MarkerUI.Instance.UpdateJobListingParameters(jobListing);

            MarkerManager.Instance.currentMarker = this;
            MarkerManager.Instance.currentGridPrefab = this.gameObject.GetComponentInParent<GridPrefab>();
            //markerOpen = true;
            //DecreaseGridPrefabMarkerCount();
            MarkerUI.Instance.OpenMarkerInfoPanel();
        }
    }

    public void DecreaseGridPrefabMarkerCount()
    {
        GetComponentInParent<GridPrefab>().DecreaseMarkerCount();
    }
}