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

        // 🔒 Sécurité au démarrage (comportement existant)
        if (panel != null)
            panel.SetActive(false);
    }

    // =====================================================
    // 👉 OUVERTURE UI (BLOQUE LE MONDE)
    // =====================================================
    public void Open(Recruitable recruit)
    {
        if (recruit == null)
            return;

        // =====================================================
        // 🔴 BLOCAGE SI TROP LOIN (LOGIQUE EXISTANTE)
        // =====================================================
        Companion companion = recruit.GetComponent<Companion>();
        if (companion != null && !companion.IsPlayerInInteractionRange())
        {
            if (InteractionFeedback.Instance != null)
            {
                InteractionFeedback.Instance.ShowTooFar();
            }
            return; // ⛔ UI jamais ouverte
        }
        // =====================================================

        currentRecruit = recruit;

        // 🔒 BLOCAGE CENTRALISÉ DES INPUTS MONDE
        UIState.OpenModal();

        if (hudPanel != null)
            hudPanel.SetActive(false);

        Unit unit = recruit.GetComponent<Unit>();
        if (nameText != null)
            nameText.text = unit != null ? unit.unitName : "Unknown";

        if (costText != null)
            costText.text = $"Cost: {recruit.recruitCost} gold";

        UpdateGoldText();

        if (panel != null)
            panel.SetActive(true);
    }

    // =====================================================
    // 👉 FERMETURE UI (DÉBLOQUE LE MONDE)
    // =====================================================
    public void Close()
    {
        if (panel != null)
            panel.SetActive(false);

        // 🔓 RESTITUTION DE L'ÉTAT PHYSIQUE DU PERSONNAGE (EXISTANT)
        if (currentRecruit != null)
        {
            currentRecruit.RestorePhysics();
        }

        currentRecruit = null;

        if (hudPanel != null)
            hudPanel.SetActive(true);

        // 🔓 RESTITUTION DES INPUTS MONDE
        UIState.CloseModal();
    }

    // =====================================================
    // 👉 CONFIRMATION DU RECRUTEMENT (INCHANGÉ)
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
    // 👉 MISE À JOUR DE L’OR (INCHANGÉ)
    // =====================================================
    void UpdateGoldText()
    {
        if (goldText != null && PlayerResources.Instance != null)
        {
            goldText.text = $"Gold: {PlayerResources.Instance.gold}";
        }
    }
}