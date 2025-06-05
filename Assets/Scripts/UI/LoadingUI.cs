using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LoadingUI : MonoBehaviour
{
    #region Singleton

    public static LoadingUI Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    #endregion

    [SerializeField] GameObject loadingBgObject;
    public GameObject loadingBarObject;
    public Image loadingBarFill;

    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] float fadeDuration = 0.3f;

    public void Show()
    {
        loadingBgObject.SetActive(true);
        loadingBarFill.fillAmount = 0f;

        canvasGroup.alpha = 0f;
        StartCoroutine(FadeCanvasGroup(canvasGroup, 0f, 1f, fadeDuration));
    }

    public void Hide()
    {
        //StartCoroutine(FadeAndDestroy());
        StartCoroutine(DelayedHide(2f));
    }

    IEnumerator DelayedHide(float time)
    {
        yield return new WaitForSeconds(time);
        StartCoroutine(FadeAndDestroy(1f));
    }

    public void UpdateProgress(float progress)
    {
        if (loadingBarFill != null)
            loadingBarFill.fillAmount = progress;
    }

    IEnumerator FadeCanvasGroup(CanvasGroup group, float from, float to, float duration)
    {
        float timer = 0f;
        group.alpha = from;

        while(timer < duration)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / duration);
            group.alpha = Mathf.Lerp(from, to, t);
            yield return null;
        }

        group.alpha = to;
    }

    IEnumerator FadeAndDestroy(float time)
    {
        yield return FadeCanvasGroup(canvasGroup, 1f, 0f, time);


        if (loadingBarObject != null) loadingBarObject.SetActive(false);

        if (Instance == this)
            Instance = null;

        Destroy(gameObject);
    }
}