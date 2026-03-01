using UnityEngine;
using UnityEngine.AI;

public class CombatController : MonoBehaviour
{
    // =====================================================
    // 🔥 API EXISTANTE (NE PAS CASSER)
    // =====================================================
    public enum CombatState
    {
        Idle,
        MovingToTarget,
        Attacking
    }

    public CombatState State { get; private set; } = CombatState.Idle;

    public CombatTarget CurrentTarget => currentTarget;

    // =====================================================
    // COMBAT PARAMS
    // =====================================================
    public float attackRange = 1.8f;
    public float attackDamage = 20f;
    public float attackCooldown = 1.2f;
    public float rotationSpeed = 10f;

    private CombatTarget currentTarget;
    private NavMeshAgent agent;
    private Animator animator;

    private float lastAttackTime;
    private bool combatActive;

    public bool HasTarget =>
        combatActive &&
        currentTarget != null &&
        currentTarget.IsAlive;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        if (!combatActive)
        {
            State = CombatState.Idle;
            return;
        }

        if (currentTarget == null || !currentTarget.IsAlive)
        {
            CancelCombat();
            return;
        }

        float dist = Vector3.Distance(
            transform.position,
            currentTarget.transform.position
        );

        if (dist > attackRange)
        {
            State = CombatState.MovingToTarget;
            MoveToTarget();
        }
        else
        {
            State = CombatState.Attacking;
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
        State = CombatState.MovingToTarget;
    }

    // =====================================================
    // 🔴 SORTIE PROPRE DU COMBAT
    // =====================================================
    public void CancelCombat()
    {
        combatActive = false;
        currentTarget = null;
        State = CombatState.Idle;

        if (agent != null && agent.enabled)
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
    // HIT FRAME
    // =====================================================
    public void ApplyDamage()
    {
        if (!HasTarget)
            return;

        Health health = currentTarget.GetComponent<Health>();
        if (health != null)
            health.TakeDamage(attackDamage, gameObject);
    }
}