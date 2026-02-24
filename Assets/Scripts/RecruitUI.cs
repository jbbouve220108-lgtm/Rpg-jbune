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

        // 🔒 Sécurité au démarrage
        panel.SetActive(false);
        UIState.IsModalOpen = false;
    }

    // 👉 OUVERTURE UI (INCHANGÉE FONCTIONNELLEMENT)
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

    // 👉 FERMETURE UI (AJOUT : restitution physique)
    public void Close()
    {
        panel.SetActive(false);

        // 🔓 RESTITUTION DE L'ÉTAT PHYSIQUE DU PERSONNAGE
        if (currentRecruit != null)
        {
            currentRecruit.RestorePhysics();
        }

        currentRecruit = null;

        if (hudPanel != null)
            hudPanel.SetActive(true);

        UIState.IsModalOpen = false;
    }

    // 👉 Bouton "Recruter" (INCHANGÉ)
    public void ConfirmRecruit()
    {
        if (currentRecruit != null)
        {
            currentRecruit.Recruit();
            UpdateGoldText();
        }
    }

    // 👉 Mise à jour de l’or (INCHANGÉ)
    void UpdateGoldText()
    {
        if (goldText != null && PlayerResources.Instance != null)
        {
            goldText.text = $"Gold: {PlayerResources.Instance.gold}";
        }
    }
}