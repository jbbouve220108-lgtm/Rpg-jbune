using UnityEngine;

public class MerchantUI : MonoBehaviour
{
    public static MerchantUI Instance;

    public GameObject panel;
    private Merchant currentMerchant;

    void Awake()
    {
        Instance = this;
        panel.SetActive(false);
    }

    public void Open(Merchant merchant)
    {
        currentMerchant = merchant;
        panel.SetActive(true);
    }

    public void Close()
    {
        currentMerchant = null;
        panel.SetActive(false);
    }

    public void BuyFood()
    {
        if (currentMerchant == null)
            return;

        currentMerchant.BuyFood();
    }
}