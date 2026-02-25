using UnityEngine;

[System.Serializable]
public class Stat
{
    [Header("Value")]
    public int value = 0;

    [Header("Progression")]
    public float progress = 0f;   // ⚠️ utilisé par RandomizeStatsOnSpawn

    [Header("Config")]
    public int maxValue = 100;

    public void AddValue(int amount)
    {
        value = Mathf.Clamp(value + amount, 0, maxValue);
    }

    public void SetValue(int newValue)
    {
        value = Mathf.Clamp(newValue, 0, maxValue);
    }

    public void AddProgress(float amount)
    {
        progress += amount;
    }

    public void ResetProgress()
    {
        progress = 0f;
    }
}