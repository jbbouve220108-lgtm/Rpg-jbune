using UnityEngine;
using TMPro;

public class RecruitUI : MonoBehaviour
{
    public static RecruitUI Instance { get; private set; }

    [Header("UI")]
    public GameObject panel;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI costText;

    [Header("HUD to hide")]
    [SerializeField] private GameObject hudPanel;

    private Recruitable currentRecruit;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        panel.SetActive(false);
    }

    public void Open(Recruitable recruit)
    {
        if (recruit == null)
            return;

        currentRecruit = recruit;

        if (hudPanel != null)
            hudPanel.SetActive(false);

        Unit unit = recruit.GetComponent<Unit>();
        nameText.text = unit != null ? unit.unitName : "Unknown";
        costText.text = $"Cost: {recruit.recruitCost} gold";

        panel.SetActive(true);
    }

    public void Close()
    {
        panel.SetActive(false);
        currentRecruit = null;

        if (hudPanel != null)
            hudPanel.SetActive(true);
    }

    public void ConfirmRecruit()
    {
        if (currentRecruit != null)
            currentRecruit.Recruit();
    }
}