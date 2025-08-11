using UnityEngine;
using UnityEngine.UI;

public class BurnoutMeter : MonoBehaviour
{
    #region Singleton
    public static BurnoutMeter Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    #endregion

    public Image fill;
    public float maxHealth = 100;
    float currentHealth;
    float damage = 1f;

    float maxTime = 1;
    float timer = 0;

    private void Start()
    {
        currentHealth = maxHealth;
        UpdateBurnoutMeter();
    }

    private void Update()
    {
        if(timer < maxTime)
        {
            timer += Time.deltaTime;
        }
        else
        {
            TakeDamage(damage);
            timer = 0;
        }
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if(currentHealth <= 0)
        {
            currentHealth = 0;
            MatchResultUI.Instance.ShowGameOverScreen();
            PauseManager.Instance.PauseGame();
        }

        UpdateBurnoutMeter();
    }

    public void Heal(float amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateBurnoutMeter();
    }

    void UpdateBurnoutMeter()
    {
        float fillAmount = currentHealth / maxHealth;
        fill.fillAmount = fillAmount;
    }
}