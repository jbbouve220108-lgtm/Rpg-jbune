using UnityEngine;

public class MerchantUI : MonoBehaviour
{
    public static MerchantUI Instance { get; private set; }

    [Header("UI")]
    [SerializeField] private GameObject panel;

    [Header("Panels to block")]
    public CanvasGroup[] panelsToBlock;

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

        foreach (var p in panelsToBlock)
            UIBlocker.Instance?.Block(p);

        panel.SetActive(true);
    }

    public void Close()
    {
        currentMerchant = null;
        panel.SetActive(false);

        foreach (var p in panelsToBlock)
            UIBlocker.Instance?.Unblock(p);
    }

    public void BuyFood()
    {
        if (currentMerchant != null)
            currentMerchant.BuyFood();
    }
}