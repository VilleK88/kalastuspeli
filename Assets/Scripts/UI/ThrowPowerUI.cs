using UnityEngine;
using UnityEngine.UI;

public class ThrowPowerUI : MonoBehaviour
{
    #region Singleton
    public static ThrowPowerUI Instance { get; private set; }
    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }
    #endregion

    public Image bg;
    public Image fill;

    public void UpdatePowerBar(float current, float max)
    {
        float t = Mathf.Clamp01(current / max);
        fill.fillAmount = t;

        bool show = current > 0f;
        bg.enabled = show;
        fill.enabled = show;
    }

    public void ResetPowerBar()
    {
        fill.fillAmount = 0f;
        fill.enabled = false;
        bg.enabled = false;
    }
}