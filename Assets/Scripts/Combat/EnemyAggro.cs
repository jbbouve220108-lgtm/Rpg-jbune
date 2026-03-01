using UnityEngine;

public class EnemyAggro : MonoBehaviour
{
    [Header("Aggro")]
    public float aggroRadius = 6f;
    public float aggroCheckInterval = 0.5f;

    private CombatController combat;
    private float lastCheckTime;

    void Awake()
    {
        combat = GetComponent<CombatController>();
    }

    void Update()
    {
        if (combat == null)
            return;

        // 🔒 si l’ennemi a déjà une cible vivante → on ne fait RIEN
        if (combat.HasTarget)
            return;

        if (Time.time - lastCheckTime < aggroCheckInterval)
            return;

        lastCheckTime = Time.time;

        TryAcquireTarget();
    }

    // =====================================================
    // LOGIQUE AGGRO ENNEMIE
    // =====================================================
    void TryAcquireTarget()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, aggroRadius);

        CombatTarget bestTarget = null;
        float bestDist = float.MaxValue;

        foreach (var hit in hits)
        {
            // 🎯 joueur
            if (hit.CompareTag("Player"))
            {
                bestTarget = hit.GetComponent<CombatTarget>();
                break; // priorité absolue
            }

            // 🎯 compagnon recruté
            Companion comp = hit.GetComponent<Companion>();
            if (comp != null && comp.isRecruited)
            {
                CombatTarget t = hit.GetComponent<CombatTarget>();
                if (t == null || !t.IsAlive)
                    continue;

                float d = Vector3.Distance(transform.position, hit.transform.position);
                if (d < bestDist)
                {
                    bestDist = d;
                    bestTarget = t;
                }
            }
        }

        if (bestTarget != null)
        {
            combat.SetAttackTarget(bestTarget);
        }
    }
}