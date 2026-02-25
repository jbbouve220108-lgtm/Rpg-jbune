using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StatRowUI : MonoBehaviour
{
    [Header("Texts")]
    public TextMeshProUGUI labelText;
    public TextMeshProUGUI currentValueText;
    public TextMeshProUGUI nextValueText;

    [Header("Bar")]
    public Image barFill;

    [Header("Config")]
    public int maxStatValue = 100;

    // =====================================================
    // APPEL PUBLIC
    // =====================================================
    public void SetStat(string statName, int currentValue)
    {
        int nextValue = GetNextThreshold(currentValue);

        currentValue = Mathf.Clamp(currentValue, 0, maxStatValue);
        nextValue = Mathf.Clamp(nextValue, 1, maxStatValue);

        if (labelText != null)
            labelText.text = statName;

        if (currentValueText != null)
            currentValueText.text = currentValue.ToString();

        if (nextValueText != null)
            nextValueText.text = nextValue.ToString();

        if (barFill != null)
        {
            float fill = currentValue / (float)nextValue;
            barFill.fillAmount = Mathf.Clamp01(fill);
        }
    }

    // =====================================================
    // PROGRESSION SIMPLE (Kenshi-like)
    // =====================================================
    private int GetNextThreshold(int current)
    {
        return ((current / 10) + 1) * 10;
    }
}