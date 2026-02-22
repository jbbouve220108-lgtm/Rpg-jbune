using UnityEngine;
using TMPro;

public class RecruitUI : MonoBehaviour
{
    public static RecruitUI Instance { get; private set; }

    [Header("UI")]
    public GameObject panel;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI costText;

    [Header("Panels to block")]
    public CanvasGroup[] panelsToBlock;

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
        currentRecruit = recruit;

        // 🔒 Bloque UNIQUEMENT les panels choisis
        foreach (var p in panelsToBlock)
            UIBlocker.Instance?.Block(p);

        Unit unit = recruit.GetComponent<Unit>();
        nameText.text = unit != null ? unit.unitName : "Unknown";
        costText.text = $"Cost: {recruit.recruitCost} gold";

        panel.SetActive(true);
    }

    public void Close()
    {
        panel.SetActive(false);
        currentRecruit = null;

        // 🔓 Débloque les panels
        foreach (var p in panelsToBlock)
            UIBlocker.Instance?.Unblock(p);
    }

    public void ConfirmRecruit()
    {
        if (currentRecruit != null)
            currentRecruit.Recruit();
    }
}