using UnityEngine;

public class AutoDefense : MonoBehaviour
{
    [Header("Auto Defense")]
    public float defenseRadius = 3f;
    public float checkInterval = 0.4f;

    private CombatController combat;
    private Companion companion;

    private float lastCheckTime;

    void Awake()
    {
        combat = GetComponent<CombatController>();
        companion = GetComponent<Companion>();
    }

    void Update()
    {
        if (combat == null)
            return;

        // 🔓 NE JAMAIS BLOQUER SI LA CIBLE EST MORTE
        if (combat.HasTarget && combat.HasTarget)
        {
            // CombatController gère la sortie de combat
            return;
        }

        // ⏱️ Limitation des checks
        if (Time.time - lastCheckTime < checkInterval)
            return;

        lastCheckTime = Time.time;

        // 🧍 Compagnon : auto-aggro seulement si IDLE
        if (companion != null && companion.isFollowing)
            return;

        TryAutoAggro();
    }

    // =====================================================
    // AGGRO PASSIVE (IDLE UNIQUEMENT)
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
    // 🔥 RIPOSTE IMMÉDIATE SI ATTAQUÉ
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