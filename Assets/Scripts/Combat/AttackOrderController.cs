using UnityEngine;
using System.Collections.Generic;

public class AttackOrderController : MonoBehaviour
{
    void Update()
    {
        if (UIState.IsModalOpen)
            return;

        HandleLeftClick();
    }

    // =====================================================
    // 🖱️ LEFT CLICK — ATTACK
    // =====================================================
    void HandleLeftClick()
    {
        if (!Input.GetMouseButtonUp(0))
            return;

        // 🔒 clic consommé par la sélection rectangle
        if (SelectionManager.Instance.ConsumeNextLeftClick)
        {
            SelectionManager.Instance.ConsumeNextLeftClick = false;
            return;
        }

        // Raycast sous la souris
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out RaycastHit hit))
            return;

        CombatTarget target = hit.collider.GetComponentInParent<CombatTarget>();
        if (target == null || !target.IsAlive)
            return;

        IssueAttackOrder(target);
    }

    // =====================================================
    // ⚔️ ISSUE ATTACK
    // =====================================================
    void IssueAttackOrder(CombatTarget target)
    {
        List<SelectableUnit> units = SelectionManager.Instance.GetSelectedUnits();
        if (units.Count == 0)
            return;

        foreach (var unit in units)
        {
            CombatController combat = unit.GetComponent<CombatController>();
            if (combat == null)
                continue;

            // 🔥 priorité manuelle absolue
            combat.SetAttackTarget(target);

            // 🔥 si c’est un compagnon → on coupe follow / formation
            Companion comp = unit.GetComponent<Companion>();
            if (comp != null)
            {
                comp.OnFormationOrder();
            }
        }
    }
}