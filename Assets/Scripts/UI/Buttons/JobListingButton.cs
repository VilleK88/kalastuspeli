using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using System.Collections;

public class JobListingButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler//, IPointerClickHandler
{
    public JobListing jobListing;
    public TextMeshProUGUI text;
    Vector3 originalScale;

    private void Start()
    {
        originalScale = transform.localScale;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        StartCoroutine(ScaleTo(originalScale * 1.025f, 0.2f));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        StartCoroutine(ScaleTo(originalScale, 0.2f));
    }

    /*public void OnPointerClick(PointerEventData eventData)
    {
        ShowJobListing();
    }*/

    public void ShowJobListing()
    {
        if (jobListing != null)
            MarkerUI.Instance.UpdateJobListingParameters(jobListing);

        MarkerUI.Instance.OpenMarkerInfoPanel();
    }

    IEnumerator ScaleTo(Vector3 target, float duration)
    {
        Vector3 start = transform.localScale;
        float time = 0;

        while(time < duration)
        {
            transform.localScale = Vector3.Lerp(start, target, time / duration);
            time += Time.unscaledDeltaTime;
            yield return null;
        }

        transform.localScale = target;
    }
}