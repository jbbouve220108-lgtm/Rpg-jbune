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
        panel.SetActive(false);
    }

    public void Open(Merchant merchant)
    {
        if (merchant == null)
            return;

        currentMerchant = merchant;

        UIState.IsModalOpen = true;

        if (hudPanel != null)
            hudPanel.SetActive(false);

        UpdateGoldText();
        panel.SetActive(true);
    }

    public void Close()
    {
        currentMerchant = null;
        panel.SetActive(false);

        if (hudPanel != null)
            hudPanel.SetActive(true);

        UIState.IsModalOpen = false;
    }

    public void BuyFood()
    {
        if (currentMerchant != null)
        {
            currentMerchant.BuyFood();
            UpdateGoldText(); // 🔁 refresh après achat
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