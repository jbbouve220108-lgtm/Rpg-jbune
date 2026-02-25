using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class RecruitUI : MonoBehaviour
{
    public static RecruitUI Instance;

    [Header("UI")]
    public GameObject panel;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI costText;

    [Header("HUD")]
    public GameObject hudPanel;

    [Header("Texts")]
    public TextMeshProUGUI goldText;

    [Header("Stats UI")]
    public List<StatRowUI> statRows = new List<StatRowUI>();

    private Recruitable currentRecruit;

    void Awake()
    {
        Instance = this;

        if (panel != null)
            panel.SetActive(false);
    }

    // =====================================================
    // OUVERTURE UI
    // =====================================================
    public void Open(Recruitable recruit)
    {
        if (recruit == null)
            return;

        Companion companion = recruit.GetComponent<Companion>();
        if (companion != null && !companion.IsPlayerInInteractionRange())
        {
            InteractionFeedback.Instance?.ShowTooFar();
            return;
        }

        currentRecruit = recruit;

        UIState.OpenModal();

        if (hudPanel != null)
            hudPanel.SetActive(false);

        Unit unit = recruit.GetComponent<Unit>();
        if (nameText != null)
            nameText.text = unit != null ? unit.unitName : "Unknown";

        if (costText != null)
            costText.text = $"Cost: {recruit.recruitCost} gold";

        UpdateGoldText();
        UpdateStatsUI();

        if (panel != null)
            panel.SetActive(true);
    }

    // =====================================================
    // FERMETURE UI
    // =====================================================
    public void Close()
    {
        if (panel != null)
            panel.SetActive(false);

        if (currentRecruit != null)
            currentRecruit.RestorePhysics();

        currentRecruit = null;

        if (hudPanel != null)
            hudPanel.SetActive(true);

        UIState.CloseModal();
    }

    // =====================================================
    // CONFIRMATION DU RECRUTEMENT
    // =====================================================
    public void ConfirmRecruit()
    {
        if (currentRecruit != null)
        {
            currentRecruit.Recruit();
            UpdateGoldText();
        }
    }

    // =====================================================
    // OR
    // =====================================================
    void UpdateGoldText()
    {
        if (goldText != null && PlayerResources.Instance != null)
            goldText.text = $"Gold: {PlayerResources.Instance.gold}";
    }

    // =====================================================
    // STATS (LECTURE DE Stat -> int)
    // =====================================================
    void UpdateStatsUI()
    {
        if (currentRecruit == null)
            return;

        CharacterStats stats = currentRecruit.GetComponent<CharacterStats>();
        if (stats == null)
            return;

        foreach (var row in statRows)
        {
            if (row == null)
                continue;

            switch (row.name)
            {
                case "Force":
                    row.SetStat("Force", stats.force.value);
                    break;

                case "Athletisme":
                    row.SetStat("Athlétisme", stats.athletisme.value);
                    break;

                case "Resistance":
                    row.SetStat("Résistance", stats.resistance.value);
                    break;

                case "Precision":
                    row.SetStat("Précision", stats.precision.value);
                    break;

                case "Commandement":
                    row.SetStat("Commandement", stats.commandement.value);
                    break;

                case "Charisme":
                    row.SetStat("Charisme", stats.charisme.value);
                    break;

                case "Chance":
                    row.SetStat("Chance", stats.chance.value);
                    break;

                case "Commerce":
                    row.SetStat("Commerce", stats.commerce.value);
                    break;

                case "Artisanat":
                    row.SetStat("Artisanat", stats.artisanat.value);
                    break;

                case "Bucheron":
                    row.SetStat("Bûcheron", stats.bucheron.value);
                    break;

                case "Mineur":
                    row.SetStat("Mineur", stats.mineur.value);
                    break;
            }
        }
    }
}