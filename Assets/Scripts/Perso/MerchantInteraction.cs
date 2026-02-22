using UnityEngine;

public class MerchantClick : MonoBehaviour
{
    void Update()
    {
        if (!Input.GetMouseButtonDown(0))
            return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out RaycastHit hit))
            return;

        Merchant merchant = hit.collider.GetComponentInParent<Merchant>();
        if (merchant)
        {
            merchant.OnClicked();
        }
    }
}