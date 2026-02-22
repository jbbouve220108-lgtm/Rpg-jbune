using UnityEngine;

public class MerchantUI : MonoBehaviour
{
    public static MerchantUI Instance { get; private set; }

    [Header("UI")]
    [SerializeField] private GameObject panel;

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

    public bool IsOpen()
    {
        return panel.activeSelf;
    }

    public void Open(Merchant merchant)
    {
        if (merchant == null) return;

        currentMerchant = merchant;
        panel.SetActive(true);

        Debug.Log($"Merchant UI opened with {merchant.name}");
    }

    public void Close()
    {
        currentMerchant = null;
        panel.SetActive(false);
    }

    public void BuyFood()
    {
        if (currentMerchant == null)
        {
            Debug.LogWarning("BuyFood called but no merchant set");
            return;
        }

        currentMerchant.BuyFood();
        Debug.Log("BUY FOOD CLICKED");

    }
}