using UnityEngine;

[System.Serializable]
public class Stat
{
    [Header("Value")]
    public int value = 0;

    [Header("Progression")]
    public float progress = 0f; // 0 → 1 normalisé

    [Header("Config")]
    public int maxValue = 100;

    // =====================================================
    // CONSTRUCTEURS
    // =====================================================
    public Stat() { }

    public Stat(Stat other)
    {
        if (other == null)
        {
            value = 0;
            progress = 0f;
            maxValue = 100;
            return;
        }

        value = other.value;
        progress = other.progress;
        maxValue = other.maxValue;
    }

    // =====================================================
    // API VALEUR
    // =====================================================
    public void AddValue(int amount)
    {
        value = Mathf.Clamp(value + amount, 0, maxValue);
    }

    public void SetValue(int newValue)
    {
        value = Mathf.Clamp(newValue, 0, maxValue);
    }

    // =====================================================
    // 🔥 PROGRESSION EXPONENTIELLE NORMALISÉE
    // =====================================================
    public bool AddProgressAndCheckLevelUp(float rawAmount)
    {
        if (value >= maxValue)
            return false;

        float requiredProgress = GetRequiredProgressForNextLevel();

        // 🔑 NORMALISATION (clé du fix)
        progress += rawAmount / requiredProgress;

        if (progress >= 1f)
        {
            progress -= 1f;
            value = Mathf.Clamp(value + 1, 0, maxValue);
            return true;
        }

        return false;
    }

    // =====================================================
    // COURBE EXPONENTIELLE LONG TERME
    // =====================================================
    float GetRequiredProgressForNextLevel()
    {
        int level = Mathf.Max(value, 1);

        // Courbe endgame (40–100 très long mais atteignable)
        return 1f + Mathf.Pow(level, 1.6f) * 0.15f;
    }
}