using UnityEngine;

[System.Serializable]
public class Stat
{
    [Header("Value")]
    public int value = 0;

    [Header("Progression")]
    public float progress = 0f;

    [Header("Config")]
    public int maxValue = 100;

    // =====================================================
    // CONSTRUCTEURS
    // =====================================================

    // Constructeur par défaut (Unity / Inspector)
    public Stat() { }

    // 🔥 Constructeur de copie (CLÉ DU FIX)
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
    // API PROGRESSION
    // =====================================================
    public void AddProgress(float amount)
    {
        progress += amount;
    }

    public void ResetProgress()
    {
        progress = 0f;
    }
}