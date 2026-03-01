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
        if (combat == null || combat.HasTarget)
            return;

        if (Time.time - lastCheckTime < aggroCheckInterval)
            return;

        lastCheckTime = Time.time;

        TryAcquireTarget();
    }

    void TryAcquireTarget()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, aggroRadius);

        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                TrySetTarget(hit.gameObject);
                return;
            }

            Companion comp = hit.GetComponent<Companion>();
            if (comp != null && comp.isRecruited)
            {
                TrySetTarget(hit.gameObject);
                return;
            }
        }
    }

    void TrySetTarget(GameObject obj)
    {
        CombatTarget target = obj.GetComponent<CombatTarget>();
        if (target == null || !target.IsAlive)
            return;

        combat.SetAttackTarget(target);
    }
}