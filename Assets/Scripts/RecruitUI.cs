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

    // =====================================================
    // INITIALISATION
    // =====================================================
    void Awake()
    {
        // 🔒 Singleton béton
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("[RecruitUI] Instance dupliquée détruite");
            Destroy(gameObject);
            return;
        }

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
        {
            Debug.LogWarning("[RecruitUI] Open appelé avec recruit null");
            return;
        }

        currentRecruit = recruit;

        UIState.OpenModal();

        if (hudPanel != null)
            hudPanel.SetActive(false);

        // =========================
        // NOM
        // =========================
        Unit unit = recruit.GetComponent<Unit>();
        if (nameText != null)
            nameText.text = unit != null ? unit.unitName : "Inconnu";

        // =========================
        // COÛT
        // =========================
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
        if (currentRecruit == null)
            return;

        currentRecruit.Recruit();
        UpdateGoldText();
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
    // STATS
    // =====================================================
    void UpdateStatsUI()
    {
        if (currentRecruit == null)
            return;

        CharacterStats stats = currentRecruit.GetComponent<CharacterStats>();
        if (stats == null)
        {
            Debug.LogWarning("[RecruitUI] CharacterStats manquant sur le recruit");
            return;
        }

        foreach (var row in statRows)
        {
            if (row == null)
                continue;

            // 🔥 Adaptation STRICTEMENT nécessaire
            // Chaque StatRowUI sait maintenant quelle stat afficher
            row.SetStat(stats);
        }
    }
}