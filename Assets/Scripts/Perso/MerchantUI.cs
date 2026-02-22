using UnityEngine;

public class MerchantUI : MonoBehaviour
{
    public static MerchantUI Instance { get; private set; }

    [Header("UI")]
    [SerializeField] private GameObject panel;

    [Header("HUD to hide")]
    [SerializeField] private GameObject hudPanel;

    private Merchant currentMerchant;

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

    public void Open(Merchant merchant)
    {
        if (merchant == null)
            return;

        currentMerchant = merchant;

        if (hudPanel != null)
            hudPanel.SetActive(false);

        panel.SetActive(true);
    }

    public void Close()
    {
        currentMerchant = null;

        panel.SetActive(false);

        if (hudPanel != null)
            hudPanel.SetActive(true);
    }

    public void BuyFood()
    {
        if (currentMerchant != null)
            currentMerchant.BuyFood();
    }
}