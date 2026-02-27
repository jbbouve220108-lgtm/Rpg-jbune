using UnityEngine;
using System.Collections.Generic;

public class UIPlayerStats : MonoBehaviour
{
    [Header("UI")]
    public GameObject panel;

    [Header("HUD")]
    public GameObject hudPanel;   // 🔹 AJOUT

    [Header("Stats UI")]
    public List<StatRowUI> statRows = new List<StatRowUI>();

    private CharacterStats playerStats;
    private bool isOpen = false;

    // =====================================================
    // UNITY
    // =====================================================
    void Awake()
    {
        if (panel != null)
            panel.SetActive(false);

        statRows.Clear();
        statRows.AddRange(GetComponentsInChildren<StatRowUI>(true));
    }

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            playerStats = player.GetComponent<CharacterStats>();
    }

    // =====================================================
    // OUVERTURE / FERMETURE
    // =====================================================
    public void TogglePanel()
    {
        if (panel == null)
            return;

        if (panel.activeSelf)
            Close();
        else
            Open();
    }

    public void Open()
    {
        if (panel == null)
            return;

        CloseAllOtherUI();

        // 🔒 HUD OFF
        if (hudPanel != null)
            hudPanel.SetActive(false);

        panel.SetActive(true);
        isOpen = true;

        UIState.OpenModal();
        RefreshStats();
    }

    public void Close()
    {
        if (panel == null)
            return;

        panel.SetActive(false);
        isOpen = false;

        // 🔓 HUD ON
        if (hudPanel != null)
            hudPanel.SetActive(true);

        UIState.CloseModal();
    }

    // =====================================================
    // RAFRAÎCHISSEMENT LIVE
    // =====================================================
    void Update()
    {
        if (!isOpen || playerStats == null)
            return;

        RefreshStats();
    }

    void RefreshStats()
    {
        foreach (var row in statRows)
        {
            if (row != null)
                row.SetStat(playerStats);
        }
    }

    // =====================================================
    // FERMETURE DES AUTRES UI
    // =====================================================
    void CloseAllOtherUI()
    {
        UICompanions companionsUI = FindAnyObjectByType<UICompanions>();
        if (companionsUI != null)
            companionsUI.ClosePanel();

        RecruitUI recruitUI = FindAnyObjectByType<RecruitUI>();
        if (recruitUI != null)
            recruitUI.Close();
    }
}