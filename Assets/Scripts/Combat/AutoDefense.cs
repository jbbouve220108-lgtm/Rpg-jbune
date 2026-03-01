using UnityEngine;

public class AutoDefense : MonoBehaviour
{
    [Header("Auto Defense")]
    public float aggroRadius = 6f;
    public float assistRadius = 5f;
    public float checkInterval = 0.4f;

    private CombatController combat;
    private Unit unit;

    // 🔥 mémoire de l’attaquant
    private CombatTarget lastAttacker;
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

        // 🔴 PRIORITÉ ABSOLUE — JE SUIS ATTAQUÉ
        if (TryFightMyAttacker())
            return;

        // 🔒 déjà en combat → on ne change pas
        if (combat.HasTarget)
            return;

        // 🟠 PRIORITÉ 2 — ENTRAIDE
        if (TryAssistAlly())
            return;

        // 🟢 PRIORITÉ 3 — AGGRO INITIALE
        TryInitialAggro();
    }

    // =====================================================
    // 🔴 RIPOSTE ABSOLUE
    // =====================================================
    bool TryFightMyAttacker()
    {
        if (lastAttacker == null || !lastAttacker.IsAlive)
        {
            lastAttacker = null;
            return false;
        }

        if (combat.CurrentTarget != lastAttacker)
            combat.SetAttackTarget(lastAttacker);

        return true;
    }

    // =====================================================
    // 🟠 ENTRAIDE
    // =====================================================
    bool TryAssistAlly()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, assistRadius);

        foreach (var hit in hits)
        {
            CombatController otherCombat = hit.GetComponent<CombatController>();
            if (otherCombat == null || !otherCombat.HasTarget)
                continue;

            CombatTarget allyTarget = otherCombat.CurrentTarget;
            if (allyTarget == null)
                continue;

            // 🔒 la cible doit être un ALLIÉ
            if (!IsAlly(allyTarget.gameObject))
                continue;

            CombatTarget attacker = hit.GetComponent<CombatTarget>();
            if (attacker == null || !attacker.IsAlive)
                continue;

            if (!IsEnemy(attacker.gameObject))
                continue;

            combat.SetAttackTarget(attacker);
            return true;
        }

        return false;
    }

    // =====================================================
    // 🟢 AGGRO INITIALE
    // =====================================================
    void TryInitialAggro()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, aggroRadius);

        CombatTarget closest = null;
        float bestDist = float.MaxValue;

        foreach (var hit in hits)
        {
            CombatTarget target = hit.GetComponent<CombatTarget>();
            if (target == null || !target.IsAlive)
                continue;

            if (!IsEnemy(hit.gameObject))
                continue;

            float dist = Vector3.Distance(transform.position, hit.transform.position);
            if (dist < bestDist)
            {
                bestDist = dist;
                closest = target;
            }
        }

        if (closest != null)
            combat.SetAttackTarget(closest);
    }

    // =====================================================
    // 🔥 APPELÉ PAR Health
    // =====================================================
    public void OnAttacked(GameObject attacker)
    {
        if (attacker == null)
            return;

        CombatTarget target = attacker.GetComponent<CombatTarget>();
        if (target == null || !target.IsAlive)
            return;

        if (!IsEnemy(attacker))
            return;

        lastAttacker = target;
    }

    // =====================================================
    // 🧠 LOGIQUE D’ALLIANCE (LA CLÉ)
    // =====================================================
    bool IsAlly(GameObject other)
    {
        // joueur toujours allié
        if (other.CompareTag("Player"))
            return true;

        // compagnon recruté = allié
        Companion comp = other.GetComponent<Companion>();
        if (comp != null && comp.isRecruited)
            return true;

        return false;
    }

    bool IsEnemy(GameObject other)
    {
        // ennemi explicite
        EnemyAggro enemy = other.GetComponent<EnemyAggro>();
        if (enemy != null)
            return true;

        return false;
    }
}