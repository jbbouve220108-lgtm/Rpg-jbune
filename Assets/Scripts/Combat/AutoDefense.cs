using UnityEngine;

public class AutoDefense : MonoBehaviour
{
    [Header("Auto Defense")]
    public float defenseRadius = 3f;
    public float checkInterval = 0.4f;

    private CombatController combat;
    private Companion companion;
    private Unit unit;

    private float lastCheckTime;

    void Awake()
    {
        combat = GetComponent<CombatController>();
        companion = GetComponent<Companion>();
        unit = GetComponent<Unit>();
    }

    void Update()
    {
        if (combat == null || unit == null)
            return;

        // ⏱️ throttling
        if (Time.time - lastCheckTime < checkInterval)
            return;

        lastCheckTime = Time.time;

        // =====================================================
        // 👤 JOUEUR : JAMAIS D’AUTO-AGGRO
        // =====================================================
        if (unit.unitType == UnitType.Player)
            return;

        // =====================================================
        // 🧍 COMPAGNON : auto-aggro UNIQUEMENT si idle
        // =====================================================
        if (companion != null && companion.isFollowing)
            return;

        TryAutoAggro();
    }

    // =====================================================
    // AUTO-AGGRO PASSIF
    // =====================================================
    void TryAutoAggro()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, defenseRadius);

        foreach (var hit in hits)
        {
            EnemyAggro enemy = hit.GetComponent<EnemyAggro>();
            if (enemy == null)
                continue;

            CombatTarget target = hit.GetComponent<CombatTarget>();
            if (target == null || !target.IsAlive)
                continue;

            combat.SetAttackTarget(target);
            return;
        }
    }

    // =====================================================
    // 🔥 RIPOSTE SI ATTAQUÉ (JOUEUR + COMPAGNON)
    // =====================================================
    public void OnAttacked(GameObject attacker)
    {
        if (combat == null)
            return;

        CombatTarget target = attacker.GetComponent<CombatTarget>();
        if (target == null || !target.IsAlive)
            return;

        combat.SetAttackTarget(target);
    }
}