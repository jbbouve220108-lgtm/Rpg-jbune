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
        BindPlayerHealth();
        UpdateResources();
        UpdateHealth();
    }

    void OnDestroy()
    {
        UnbindPlayerHealth();
    }

    void Update()
    {
        UpdateResources();
    }

    // =====================================================
    // 🔗 BIND HEALTH
    // =====================================================
    void BindPlayerHealth()
    {
        if (playerHealth == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                playerHealth = player.GetComponent<Health>();
        }

        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged += OnHealthChanged;
        }
    }

    void UnbindPlayerHealth()
    {
        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged -= OnHealthChanged;
        }
    }

    // =====================================================
    // CALLBACK
    // =====================================================
    void OnHealthChanged(float current, float max)
    {
        UpdateHealth();
    }

    // =====================================================
    // RESOURCES
    // =====================================================
    void UpdateResources()
    {
        if (PlayerResources.Instance == null)
            return;

        goldText.text = $"Gold: {PlayerResources.Instance.gold}";
        foodText.text = $"Food: {PlayerResources.Instance.food}";
    }

    // =====================================================
    // HEALTH
    // =====================================================
    void UpdateHealth()
    {
        if (playerHealth == null)
            return;

        healthText.text =
            $"HP: {Mathf.Ceil(playerHealth.currentHealth)} / {playerHealth.maxHealth}";
    }
}