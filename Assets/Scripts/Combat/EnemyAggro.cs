using UnityEngine;

public class EnemyAggro : MonoBehaviour
{
    [Header("Enemy Aggro")]
    public float aggroRadius = 6f;     // détection générale
    public float assistRadius = 5f;    // entraide
    public float checkInterval = 0.5f;

    [Header("Smart Retargeting (AJOUT)")]
    public float retargetDistanceThreshold = 1.2f; // différence significative

    private CombatController combat;
    private Unit unit;

    private float lastCheckTime;

    void Awake()
    {
        combat = GetComponent<CombatController>();
        unit = GetComponent<Unit>();
    }

    void Update()
    {
        if (combat == null || unit == null)
            return;

        if (Time.time - lastCheckTime < checkInterval)
            return;

        lastCheckTime = Time.time;

        // 🔴 PRIORITÉ 1 — JE SUIS ATTAQUÉ
        if (TryFightMyAttacker())
            return;

        // 🔒 si déjà en combat → on peut réévaluer intelligemment
        if (combat.HasTarget)
        {
            TrySwitchToCloserThreat();
            return;
        }

        // 🟠 PRIORITÉ 2 — ENTRAIDE
        if (TryAssistAlly())
            return;

        // 🟢 PRIORITÉ 3 — AGGRO INITIALE
        TryInitialAggro();
    }

    // =====================================================
    // 🔴 PRIORITÉ ABSOLUE — RIPOSTE
    // =====================================================
    bool TryFightMyAttacker()
    {
        Collider[] hits = Physics.OverlapSphere(
            transform.position,
            aggroRadius
        );

        foreach (var hit in hits)
        {
            CombatController otherCombat =
                hit.GetComponent<CombatController>();

            if (otherCombat == null)
                continue;

            // est-ce qu'il m'attaque ?
            if (otherCombat.CurrentTarget ==
                GetComponent<CombatTarget>())
            {
                CombatTarget attacker =
                    hit.GetComponent<CombatTarget>();

                if (attacker != null &&
                    attacker.IsAlive &&
                    IsEnemy(hit.gameObject))
                {
                    combat.SetAttackTarget(attacker);
                    return true;
                }
            }
        }

        return false;
    }

    // =====================================================
    // 🟠 PRIORITÉ 2 — ENTRAIDE
    // =====================================================
    bool TryAssistAlly()
    {
        Collider[] hits = Physics.OverlapSphere(
            transform.position,
            assistRadius
        );

        foreach (var hit in hits)
        {
            CombatController otherCombat =
                hit.GetComponent<CombatController>();

            if (otherCombat == null || !otherCombat.HasTarget)
                continue;

            CombatTarget allyTarget =
                otherCombat.CurrentTarget;

            if (allyTarget == null)
                continue;

            // un allié est attaqué
            if (IsAlly(allyTarget.gameObject))
            {
                CombatTarget attacker =
                    hit.GetComponent<CombatTarget>();

                if (attacker != null &&
                    attacker.IsAlive &&
                    IsEnemy(hit.gameObject))
                {
                    combat.SetAttackTarget(attacker);
                    return true;
                }
            }
        }

        return false;
    }

    // =====================================================
    // 🟢 PRIORITÉ 3 — AGGRO INITIALE
    // =====================================================
    void TryInitialAggro()
    {
        Collider[] hits = Physics.OverlapSphere(
            transform.position,
            aggroRadius
        );

        CombatTarget closest = null;
        float bestDist = float.MaxValue;

        foreach (var hit in hits)
        {
            CombatTarget target =
                hit.GetComponent<CombatTarget>();

            if (target == null || !target.IsAlive)
                continue;

            if (!IsEnemy(hit.gameObject))
                continue;

            float dist = Vector3.Distance(
                transform.position,
                hit.transform.position
            );

            if (dist < bestDist)
            {
                bestDist = dist;
                closest = target;
            }
        }

        if (closest != null)
        {
            combat.SetAttackTarget(closest);
        }
    }

    // =====================================================
    // 🆕 RÉÉVALUATION INTELLIGENTE (AJOUT)
    // =====================================================
    void TrySwitchToCloserThreat()
    {
        CombatTarget current = combat.CurrentTarget;
        if (current == null)
            return;

        float currentDist = Vector3.Distance(
            transform.position,
            current.transform.position
        );

        Collider[] hits = Physics.OverlapSphere(
            transform.position,
            assistRadius
        );

        CombatTarget bestCandidate = null;
        float bestDist = currentDist;

        foreach (var hit in hits)
        {
            CombatTarget target = hit.GetComponent<CombatTarget>();
            if (target == null || !target.IsAlive)
                continue;

            if (!IsEnemy(hit.gameObject))
                continue;

            if (target == current)
                continue;

            float dist = Vector3.Distance(
                transform.position,
                hit.transform.position
            );

            // seulement si nettement plus proche
            if (dist + retargetDistanceThreshold < bestDist)
            {
                bestDist = dist;
                bestCandidate = target;
            }
        }

        if (bestCandidate != null)
        {
            combat.SetAttackTarget(bestCandidate);
        }
    }

    // =====================================================
    // 🧠 FILTRES
    // =====================================================
    bool IsEnemy(GameObject other)
    {
        Unit otherUnit = other.GetComponent<Unit>();
        if (otherUnit == null)
            return false;

        return otherUnit.unitType != unit.unitType;
    }

    bool IsAlly(GameObject other)
    {
        Unit otherUnit = other.GetComponent<Unit>();
        if (otherUnit == null)
            return false;

        return otherUnit.unitType == unit.unitType;
    }
}