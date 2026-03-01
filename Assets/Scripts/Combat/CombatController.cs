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
    private Unit myUnit;

    private float lastAttackTime;
    private bool combatActive = false;

    // =====================================================
    // 🧠 ÉTATS DE COMBAT (API COMPLÈTE)
    // =====================================================
    public enum CombatState
    {
        Idle,
        MovingToTarget,
        Attacking,
        ForcedAttack
    }

    private CombatState state = CombatState.Idle;
    public CombatState State => state;

    public bool HasTarget =>
        combatActive &&
        currentTarget != null &&
        currentTarget.IsAlive;

    public CombatTarget CurrentTarget => currentTarget;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        myUnit = GetComponent<Unit>();
    }

    void Update()
    {
        if (!combatActive || currentTarget == null)
        {
            state = CombatState.Idle;
            return;
        }

        if (!currentTarget.IsAlive)
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
            state = CombatState.MovingToTarget;
            MoveToTarget();
        }
        else
        {
            state = CombatState.Attacking;
            AttackTarget();
        }
    }

    // =====================================================
    // ⚔️ SET ATTACK TARGET (API COMPATIBLE)
    // =====================================================
    public void SetAttackTarget(CombatTarget target)
    {
        SetAttackTarget(target, false);
    }

    public void SetAttackTarget(CombatTarget target, bool forced)
    {
        if (target == null || !target.IsAlive)
            return;

        // 🔒 VERROU CAMP — JAMAIS UN ALLIÉ
        Unit targetUnit = target.GetComponent<Unit>();
        if (myUnit != null &&
            targetUnit != null &&
            myUnit.unitType == targetUnit.unitType)
        {
            return;
        }

        currentTarget = target;
        combatActive = true;
        state = forced ? CombatState.ForcedAttack : CombatState.MovingToTarget;
    }

    // =====================================================
    // 🛑 STOP COMBAT
    // =====================================================
    public void CancelCombat()
    {
        combatActive = false;
        currentTarget = null;
        state = CombatState.Idle;

        if (agent != null && agent.enabled)
        {
            agent.isStopped = false;
            agent.ResetPath();
        }
    }

    // =====================================================
    // 🚶 MOVE
    // =====================================================
    void MoveToTarget()
    {
        if (agent == null || !agent.enabled || !agent.isOnNavMesh)
            return;

        agent.isStopped = false;
        agent.SetDestination(currentTarget.transform.position);
    }

    // =====================================================
    // 🗡️ ATTACK
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
    // 🔄 ROTATION
    // =====================================================
    void RotateTowardsTarget()
    {
        Vector3 dir =
            currentTarget.transform.position - transform.position;
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
    // 💥 HIT FRAME (DOUBLE VERROU ANTI ALLIÉ)
    // =====================================================
    public void ApplyDamage()
    {
        if (!HasTarget)
            return;

        Unit targetUnit = currentTarget.GetComponent<Unit>();
        if (myUnit != null &&
            targetUnit != null &&
            myUnit.unitType == targetUnit.unitType)
        {
            CancelCombat();
            return;
        }

        Health health = currentTarget.GetComponent<Health>();
        if (health != null)
        {
            health.TakeDamage(attackDamage, gameObject);
        }
    }
}