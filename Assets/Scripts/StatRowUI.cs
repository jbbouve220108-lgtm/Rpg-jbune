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

    // =====================================================
    // APPEL PUBLIC
    // =====================================================
    public void SetStat(CharacterStats stats)
    {
        if (stats == null)
            return;

        Stat stat = GetStatFromCharacter(stats);
        if (stat == null)
            return;

        int currentValue = Mathf.Clamp(stat.value, 0, maxStatValue);
        int nextValue = Mathf.Clamp(GetNextThreshold(currentValue), 1, maxStatValue);

        if (labelText != null)
            labelText.text = GetLabel();

        if (currentValueText != null)
            currentValueText.text = currentValue.ToString();

        if (nextValueText != null)
            nextValueText.text = nextValue.ToString();

        if (barFill != null)
            barFill.fillAmount = Mathf.Clamp01(currentValue / (float)nextValue);
    }

    // =====================================================
    // RÉSOLUTION STAT
    // =====================================================
    Stat GetStatFromCharacter(CharacterStats stats)
    {
        return statType switch
        {
            StatType.Force => stats.force,
            StatType.Athletisme => stats.athletisme,
            StatType.Resistance => stats.resistance,
            StatType.Precision => stats.precision,

            StatType.Commandement => stats.commandement,
            StatType.Charisme => stats.charisme,
            StatType.Chance => stats.chance,

            StatType.Commerce => stats.commerce,
            StatType.Artisanat => stats.artisanat,
            StatType.Bucheron => stats.bucheron,
            StatType.Mineur => stats.mineur,

            _ => null
        };
    }

    string GetLabel()
    {
        return statType.ToString();
    }

    int GetNextThreshold(int current)
    {
        return ((current / 10) + 1) * 10;
    }
}