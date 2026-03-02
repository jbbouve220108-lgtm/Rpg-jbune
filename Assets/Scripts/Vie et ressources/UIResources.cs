using UnityEngine;
using TMPro;

public class UIResources : MonoBehaviour
{
    [Header("Texts")]
    public TextMeshProUGUI goldText;
    public TextMeshProUGUI foodText;
    public TextMeshProUGUI healthText;

    [Header("Player")]
    public Health playerHealth;

    void Start()
    {
        TryBindHealth();
        UpdateResources();
    }

    void OnEnable()
    {
        TryBindHealth();
    }

    void OnDisable()
    {
        UnbindHealth();
    }

    void Update()
    {
        UpdateResources();
    }

    // =====================================================
    // BIND HEALTH SAFE
    // =====================================================
    void TryBindHealth()
    {
        if (playerHealth == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                playerHealth = player.GetComponent<Health>();
        }

        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged -= UpdateHealth;
            playerHealth.OnHealthChanged += UpdateHealth;

            // 🔔 force refresh
            UpdateHealth(playerHealth.currentHealth, playerHealth.maxHealth);
        }
    }

    void UnbindHealth()
    {
        if (playerHealth != null)
            playerHealth.OnHealthChanged -= UpdateHealth;
    }

    // =====================================================
    // RESOURCES
    // =====================================================
    void UpdateResources()
    {
        if (PlayerResources.Instance == null)
            return;

        if (goldText != null)
            goldText.text = $"Gold: {PlayerResources.Instance.gold}";

        if (foodText != null)
            foodText.text = $"Food: {PlayerResources.Instance.food}";
    }

    // =====================================================
    // HEALTH HUD
    // =====================================================
    void UpdateHealth(float current, float max)
    {
        if (healthText == null)
            return;

        healthText.text = $"HP: {Mathf.Ceil(current)} / {max}";
    }
}