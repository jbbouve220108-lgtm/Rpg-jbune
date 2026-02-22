using UnityEngine;

public class Merchant : MonoBehaviour
{
    public int foodPrice = 5;
    public int foodAmount = 1;

    public void BuyFood()
    {
        if (!PlayerResources.Instance.SpendGold(foodPrice))
            return;

        PlayerResources.Instance.AddFood(foodAmount);
    }

    public void OnClicked()
    {
        MerchantUI.Instance.Open(this);
    }
}