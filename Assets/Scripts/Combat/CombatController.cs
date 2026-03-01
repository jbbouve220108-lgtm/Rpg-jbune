using UnityEngine;
using UnityEngine.AI;

public class CombatController : MonoBehaviour
{
    [Header("Combat")]
    public float attackRange = 1.8f;
    public float attackDamage = 20f;
    public float attackCooldown = 1.2f;
    public float rotationSpeed = 10f;

    private CombatTarget currentTarget;
    private NavMeshAgent agent;
    private Animator animator;

    private float lastAttackTime;

    // 🔒 état interne
    private bool combatActive = false;

    public bool HasTarget => combatActive && currentTarget != null && currentTarget.IsAlive;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        if (!combatActive)
            return;

        // 🔥 cible morte → fin combat
        if (currentTarget == null || !currentTarget.IsAlive)
        {
            CancelCombat();
            return;
        }

        float dist = Vector3.Distance(transform.position, currentTarget.transform.position);

        if (dist > attackRange)
        {
            MoveToTarget();
        }
        else
        {
            AttackTarget();
        }
    }

    // =====================================================
    // ORDRE D’ATTAQUE
    // =====================================================
    public void SetAttackTarget(CombatTarget target)
    {
        if (target == null || !target.IsAlive)
            return;

        combatActive = true;
        currentTarget = target;
    }

    // =====================================================
    // 🔴 COUPURE ABSOLUE DU COMBAT
    // =====================================================
    public void CancelCombat()
    {
        combatActive = false;
        currentTarget = null;

        if (agent != null)
        {
            agent.isStopped = false;
            agent.ResetPath();
        }
    }

    // =====================================================
    // MOVE
    // =====================================================
    void MoveToTarget()
    {
        if (agent == null || !agent.enabled || !agent.isOnNavMesh)
            return;

        agent.isStopped = false;
        agent.SetDestination(currentTarget.transform.position);
    }

    // =====================================================
    // ATTACK
    // =====================================================
    void AttackTarget()
    {
        if (agent != null)
            agent.isStopped = true;

        RotateTowardsTarget();

        if (Time.time - lastAttackTime < attackCooldown)
            return;

        lastAttackTime = Time.time;

        if (animator != null)
            animator.SetTrigger("Attack");
    }

    // =====================================================
    // ROTATION
    // =====================================================
    void RotateTowardsTarget()
    {
        Vector3 dir = currentTarget.transform.position - transform.position;
        dir.y = 0f;

        if (dir.sqrMagnitude < 0.001f)
            return;

        Quaternion rot = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            rot,
            Time.deltaTime * rotationSpeed
        );
    }

    // =====================================================
    // HIT FRAME (Animation Event)
    // =====================================================
    public void ApplyDamage()
    {
        if (!HasTarget)
            return;

        Health health = currentTarget.GetComponent<Health>();
        if (health != null)
        {
            health.TakeDamage(attackDamage, gameObject);
        }
    }
}