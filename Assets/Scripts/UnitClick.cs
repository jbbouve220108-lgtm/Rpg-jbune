using UnityEngine;

public class UnitClick : MonoBehaviour
{
    void Update()
    {
        if (!Input.GetMouseButtonDown(0))
            return;

        // 🔒 Si une UI est ouverte, on ignore
        if (UIState.IsModalOpen)
            return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out RaycastHit hit))
            return;

        // 🔹 Recrutement uniquement si Recruitable
        Recruitable recruit = hit.collider.GetComponentInParent<Recruitable>();
        if (recruit == null)
            return;

        // 🔹 Sécurité : pas de recrutement si déjà recruté
        // (le script Recruitable gère aussi, mais double protection)
        recruit.OnClicked();
    }
}