using UnityEngine;

public class UnitClick : MonoBehaviour
{
    void Update()
    {
        if (UIBlocker.Instance != null && UIBlocker.Instance.IsBlocked())
            return;

        if (!Input.GetMouseButtonDown(0))
            return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out RaycastHit hit))
            return;

        Recruitable recruit = hit.collider.GetComponentInParent<Recruitable>();
        if (recruit != null)
        {
            recruit.OnClicked();
        }
    }
}