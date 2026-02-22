using UnityEngine;
using TMPro;

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

    private Recruitable currentRecruit;

    void Awake()
    {
        Instance = this;
        panel.SetActive(false);
    }

    public void Open(Recruitable recruit)
    {
        if (recruit == null)
            return;

        currentRecruit = recruit;

        UIState.IsModalOpen = true;

        if (hudPanel != null)
            hudPanel.SetActive(false);

        Unit unit = recruit.GetComponent<Unit>();
        nameText.text = unit != null ? unit.unitName : "Unknown";
        costText.text = $"Cost: {recruit.recruitCost} gold";

        UpdateGoldText();
        panel.SetActive(true);
    }

    public void Close()
    {
        panel.SetActive(false);
        currentRecruit = null;

        if (hudPanel != null)
            hudPanel.SetActive(true);

        UIState.IsModalOpen = false;
    }

    public void ConfirmRecruit()
    {
        if (currentRecruit != null)
        {
            currentRecruit.Recruit();
            UpdateGoldText(); // 🔁 refresh après recrutement
        }
    }

    void UpdateGoldText()
    {
        if (goldText != null && PlayerResources.Instance != null)
        {
            goldText.text = $"Gold: {PlayerResources.Instance.gold}";
        }
    }
}