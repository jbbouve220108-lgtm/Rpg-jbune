using UnityEngine;

public class UnitClick : MonoBehaviour
{
    void Update()
    {
        if (!Input.GetMouseButtonDown(0))
            return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out RaycastHit hit))
            return;

        Recruitable recruit = hit.collider.GetComponentInParent<Recruitable>();
        if (recruit)
            recruit.OnClicked();
    }
}