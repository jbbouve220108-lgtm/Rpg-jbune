using UnityEngine;
using TMPro;

public class MerchantUI : MonoBehaviour
{
    public static MerchantUI Instance;

    [Header("UI")]
    public GameObject panel;

    [Header("HUD")]
    public GameObject hudPanel;

    [Header("Texts")]
    public TextMeshProUGUI goldText;

    private Merchant currentMerchant;

    void Awake()
    {
        Instance = this;

        if (panel != null)
            panel.SetActive(false);
    }

    // =====================================================
    // 👉 OUVERTURE UI MARCHAND (BLOQUE LE MONDE)
    // =====================================================
    public void Open(Merchant merchant)
    {
        if (merchant == null)
            return;

        currentMerchant = merchant;

        // 🔒 BLOCAGE CENTRALISÉ DES INPUTS MONDE
        UIState.OpenModal();

        if (hudPanel != null)
            hudPanel.SetActive(false);

        UpdateGoldText();

        if (panel != null)
            panel.SetActive(true);
    }

    // =====================================================
    // 👉 FERMETURE UI MARCHAND (DÉBLOQUE LE MONDE)
    // =====================================================
    public void Close()
    {
        currentMerchant = null;

        if (panel != null)
            panel.SetActive(false);

        if (hudPanel != null)
            hudPanel.SetActive(true);

        // 🔓 RESTITUTION DES INPUTS MONDE
        UIState.CloseModal();
    }

    // =====================================================
    // 👉 ACHAT (INCHANGÉ)
    // =====================================================
    public void BuyFood()
    {
        if (currentMerchant != null)
        {
            currentMerchant.BuyFood();
            UpdateGoldText(); // 🔁 refresh après achat
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