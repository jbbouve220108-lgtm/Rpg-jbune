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
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // 🔥 LE FIX : On ignore ce qui a été mis dans l'inspecteur 
        // et on va chercher les objets REELS dans le Canvas.
        statRows.Clear();
        statRows.AddRange(GetComponentsInChildren<StatRowUI>(true));

        if (panel != null)
            panel.SetActive(false);
    }

    public void Open(Recruitable recruit)
    {
        if (recruit == null) return;

        currentRecruit = recruit;
        UIState.OpenModal();

        if (hudPanel != null) hudPanel.SetActive(false);

        Unit unit = recruit.GetComponent<Unit>();
        if (nameText != null)
            nameText.text = unit != null ? unit.unitName : "Inconnu";

        if (costText != null)
            costText.text = $"Cost: {recruit.recruitCost} gold";

        if (panel != null) panel.SetActive(true);

        CharacterStats stats = recruit.GetComponent<CharacterStats>();
        if (stats != null)
        {
            stats.EnsureInitialized();
            foreach (var row in statRows)
            {
                if (row != null) row.SetStat(stats);
            }
        }
        UpdateGoldText();
    }

    public void Close()
    {
        if (panel != null) panel.SetActive(false);
        if (currentRecruit != null) currentRecruit.RestorePhysics();
        currentRecruit = null;
        if (hudPanel != null) hudPanel.SetActive(true);
        UIState.CloseModal();
    }

    public void ConfirmRecruit()
    {
        if (currentRecruit == null) return;
        currentRecruit.Recruit();
        UpdateGoldText();
    }

    void UpdateGoldText()
    {
        if (goldText != null && PlayerResources.Instance != null)
            goldText.text = $"Gold: {PlayerResources.Instance.gold}";
    }
}