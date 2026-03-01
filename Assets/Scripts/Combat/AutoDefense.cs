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

        // 🔒 si déjà en combat, on ne cherche rien
        if (combat.HasTarget)
            return;

        // ⏱️ limitation des checks
        if (Time.time - lastCheckTime < checkInterval)
            return;

        lastCheckTime = Time.time;

        // 🔹 si compagnon : seulement si idle
        if (companion != null && companion.isFollowing)
            return;

        TryAutoDefend();
    }

    // =====================================================
    // AUTO DEFENSE LOGIC
    // =====================================================
    void TryAutoDefend()
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

            // ⚔️ on riposte
            combat.SetAttackTarget(target);
            return;
        }
    }

    // =====================================================
    // 🔥 APPELÉ QUAND ON PREND DES DÉGÂTS
    // =====================================================
    public void OnAttacked(GameObject attacker)
    {
        if (combat == null || combat.HasTarget)
            return;

        CombatTarget target = attacker.GetComponent<CombatTarget>();
        if (target == null || !target.IsAlive)
            return;

        combat.SetAttackTarget(target);
    }
}