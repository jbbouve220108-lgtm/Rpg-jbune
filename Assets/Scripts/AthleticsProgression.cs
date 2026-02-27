using UnityEngine;

public class AthleticsProgression : MonoBehaviour
{
    [Header("Progression Settings")]
    public float baseXpRequired = 10f;
    public float exponentialFactor = 1.15f;
    public int maxLevel = 100;

    private CharacterStats stats;

    void Awake()
    {
        stats = GetComponent<CharacterStats>();
    }

    // =====================================================
    // API PUBLIQUE
    // =====================================================
    public void AddXp(float amount)
    {
        if (stats == null || stats.athletisme == null)
            return;

        Stat ath = stats.athletisme;

        if (ath.value >= maxLevel)
            return;

        ath.progress += amount;

        float xpNeeded = GetXpRequiredForNextLevel(ath.value);

        if (ath.progress >= xpNeeded)
        {
            ath.progress -= xpNeeded;
            ath.value++;
        }
    }

    public float GetXpRequiredForNextLevel(int currentLevel)
    {
        return baseXpRequired * Mathf.Pow(exponentialFactor, currentLevel);
    }
}