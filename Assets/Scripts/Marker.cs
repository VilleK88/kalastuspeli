using UnityEngine;

public class Marker : MonoBehaviour
{
    [SerializeField] GameObject circleObject;
    [SerializeField] ParticleSystem particleSystem;
    public bool canInteract;
    public IndustryType industryType;
    public IndustryData industryData;

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
        if(canInteract)
        {
            MouseManager.Instance.StartFishing();
            MarkerUI.Instance.OpenMarkerInfoPanel();
            Debug.Log("canInteract: " + canInteract);
        }
        else
        {
            Debug.Log("canInteract: " + canInteract);
        }
    }
}