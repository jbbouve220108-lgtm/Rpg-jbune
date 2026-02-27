using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StatRowUI : MonoBehaviour
{
    [Header("Config")]
    public StatType statType;
    public int maxStatValue = 100;

    [Header("Texts")]
    public TextMeshProUGUI labelText;
    public TextMeshProUGUI currentValueText;
    public TextMeshProUGUI nextValueText;

    [Header("Bar")]
    public Image barFill;

    public void SetStat(CharacterStats stats)
    {
        ResetUI();

        if (stats == null)
            return;

        Stat stat = ResolveStat(stats);
        if (stat == null)
            return;

        int currentValue = stat.value;
        int nextThreshold = GetNextThreshold(currentValue);

        if (labelText != null)
            labelText.text = statType.ToString();

        if (currentValueText != null)
            currentValueText.text = currentValue.ToString();

        if (nextValueText != null)
            nextValueText.text = nextThreshold.ToString();

        if (barFill != null)
        {
            // ✅ BARRE PROGRESSIVE VERS LE NIVEAU SUIVANT
            // stat.progress est normalisé (0 → 1)
            barFill.fillAmount = Mathf.Clamp01(stat.progress);
        }
    }

    void ResetUI()
    {
        if (labelText != null) labelText.text = "";
        if (currentValueText != null) currentValueText.text = "0";
        if (nextValueText != null) nextValueText.text = "";
        if (barFill != null) barFill.fillAmount = 0f;
    }

    Stat ResolveStat(CharacterStats stats)
    {
        switch (statType)
        {
            case StatType.Force:        return stats.force;
            case StatType.Athletisme:   return stats.athletisme;
            case StatType.Resistance:  return stats.resistance;
            case StatType.Precision:   return stats.precision;

            case StatType.Charisme:     return stats.charisme;
            case StatType.Commandement: return stats.commandement;
            case StatType.Chance:       return stats.chance;

            case StatType.Mineur:       return stats.mineur;
            case StatType.Bucheron:     return stats.bucheron;
            case StatType.Artisanat:    return stats.artisanat;
            case StatType.Commerce:     return stats.commerce;
        }
        return null;
    }

    // 🔥 Logique de palier inchangée
    int GetNextThreshold(int current)
    {
        return current + 1;
    }
}