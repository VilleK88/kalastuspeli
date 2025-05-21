using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using System.Collections;

public class GoToCity : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public City city;

    Vector3 originalScale;

    void Start()
    {
        originalScale = transform.localScale;
    }

    public void GoToThisCity()
    {
        GameManager.Instance.SetCity(city);
        SceneManager.LoadScene("3 - City");
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        StartCoroutine(ScaleTo(originalScale * 1.1f, 0.2f));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        StartCoroutine(ScaleTo(originalScale, 0.2f));
    }

    IEnumerator ScaleTo(Vector3 target, float duration)
    {
        Vector3 start = transform.localScale;
        float time = 0f;

        while(time < duration)
        {
            transform.localScale = Vector3.Lerp(start, target, time / duration);
            time += Time.unscaledDeltaTime;
            yield return null;
        }

        transform.localScale = target;
    }
}