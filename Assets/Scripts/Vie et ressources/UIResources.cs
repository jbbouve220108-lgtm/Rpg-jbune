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

    void Update()
    {
        UpdateResources();
        UpdateHealth();
    }

    void UpdateResources()
    {
        if (PlayerResources.Instance == null) return;

        goldText.text = $"Gold: {PlayerResources.Instance.gold}";
        foodText.text = $"Food: {PlayerResources.Instance.food}";
    }

    void UpdateHealth()
    {
        if (playerHealth == null) return;

        healthText.text =
            $"HP: {Mathf.Ceil(playerHealth.currentHealth)} / {playerHealth.maxHealth}";
    }
}