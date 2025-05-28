using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using System.Collections;

public class GoToCity : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public City city;
    Vector3 originalScale;
    [SerializeField] GameObject popup;
    float lastClickTime;
    const float doubleClickThreshold = 0.5f;

    void Start()
    {
        originalScale = transform.localScale;
    }

    public void GoToThisCity()
    {
        GameManager.Instance.SetCity(city);
        //SceneManager.LoadScene("3 - City");
        GameManager.Instance.LoadCityScene();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        StartCoroutine(ScaleTo(originalScale * 1.2f, 0.2f));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        popup.SetActive(false);
        StartCoroutine(ScaleTo(originalScale, 0.2f));
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        AudioManager.Instance.PlaySFX("ButtonClick");
        float timeSinceLastClick = Time.unscaledTime - lastClickTime;

        if (timeSinceLastClick <= doubleClickThreshold)
        {
            popup.SetActive(false);
            GoToThisCity();
        }
        else
            popup.SetActive(true);

        lastClickTime = Time.unscaledTime;
    }

    IEnumerator ScaleTo(Vector3 target, float duration)
    {
        Vector3 start = transform.localScale;
        float time = 0f;

        while (time < duration)
        {
            transform.localScale = Vector3.Lerp(start, target, time / duration);
            time += Time.unscaledDeltaTime;
            yield return null;
        }

        transform.localScale = target;
    }
}