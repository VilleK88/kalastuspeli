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
    [SerializeField] Image loadingBgImage;
    public GameObject loadingBarObject;
    public Image loadingBarFill;

    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] float fadeDuration = 0.3f;
    [SerializeField] Color bgVisibleColor = new Color(0f, 0f, 0f, 1f);
    [SerializeField] Color bgHiddenColor = new Color(0f, 0f, 0f, 0f);

    private void Start()
    {
        loadingBgImage = loadingBgObject.GetComponent<Image>();
    }

    public void Show()
    {
        loadingBgImage.enabled = true;
        loadingBarObject.SetActive(true);
        loadingBarFill.fillAmount = 0f;

        canvasGroup.alpha = 0f;
        StartCoroutine(FadeCanvasGroup(canvasGroup, 0f, 1f, fadeDuration));

        loadingBgImage.color = bgHiddenColor;
        StartCoroutine(FadeImageAlpha(loadingBgImage, bgHiddenColor, bgVisibleColor, fadeDuration));
    }

    public void Hide()
    {
        StartCoroutine(FadeAndDestroy());
    }

    public void UpdateProgress(float progress)
    {
        if (loadingBarFill != null)
            loadingBarFill.fillAmount = progress;
    }

    IEnumerator FadeCanvasGroup(CanvasGroup group, float from, float to, float duration)
    {
        float timer = 0f;
        while(timer < duration)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / duration);
            group.alpha = Mathf.Lerp(from, to, t);
            yield return null;
        }
        group.alpha = to;
    }

    IEnumerator FadeAndDestroy()
    {
        yield return FadeCanvasGroup(canvasGroup, 1f, 0f, fadeDuration);

        yield return FadeImageAlpha(loadingBgImage, bgVisibleColor, bgHiddenColor, fadeDuration);

        if (loadingBgImage != null) loadingBgImage.enabled = false;
        if (loadingBarObject != null) loadingBarObject.SetActive(false);

        if (Instance == this)
            Instance = null;

        Destroy(gameObject);
    }

    IEnumerator FadeImageAlpha(Image image, Color fromColor, Color toColor, float duration)
    {
        float timer = 0f;
        while(timer < duration)
        {
            timer += Time.deltaTime;
            image.color = Color.Lerp(fromColor, toColor, timer / duration);
            yield return null;
        }
        image.color = toColor;
    }
}