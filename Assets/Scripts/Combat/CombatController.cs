using UnityEngine;
using UnityEngine.AI;

public class CombatController : MonoBehaviour
{
    // =====================================================
    // API EXISTANTE
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
    // PARAMÈTRES EXISTANTS
    // =====================================================
    public float attackRange = 1.8f;
    public float attackDamage = 20f;
    public float attackCooldown = 1.2f;
    public float rotationSpeed = 10f;

    // =====================================================
    // DÉSENGAGEMENT
    // =====================================================
    [Header("Disengage")]
    public float disengageDuration = 3f;
    public float disengageDistance = 12f; // 🆕 distance de rupture

    private float disengageUntilTime = 0f;

    // =====================================================
    // INTERNES
    // =====================================================
    private CombatTarget currentTarget;
    private NavMeshAgent agent;
    private Animator animator;

    private float lastAttackTime;
    private bool combatActive;

    private Vector3 lastTargetPosition;
    private const float repathDistance = 0.5f;

    public bool HasTarget =>
        combatActive &&
        currentTarget != null &&
        currentTarget.IsAlive;

    // =====================================================
    // UNITY
    // =====================================================
    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();

        if (agent != null)
        {
            agent.updateRotation = false;
            agent.stoppingDistance = attackRange * 0.9f;
        }
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
            CancelCombatInternal(true);
            return;
        }

        // 🧨 RUPTURE PAR DISTANCE (RESTAURÉE)
        float hardDist = Vector3.Distance(
            transform.position,
            currentTarget.transform.position
        );

        if (hardDist > disengageDistance)
        {
            CancelCombatInternal(true);
            return;
        }

        float dist = hardDist;

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
    // ORDRE D’ATTAQUE (INTENTION VOLONTAIRE)
    // =====================================================
    public void SetAttackTarget(CombatTarget target)
    {
        if (target == null || !target.IsAlive)
            return;

        // 🔥 intention volontaire → annule toute fuite
        disengageUntilTime = 0f;

        combatActive = true;
        currentTarget = target;
        lastTargetPosition = Vector3.zero;

        if (agent != null && agent.enabled)
        {
            agent.isStopped = false;
            agent.stoppingDistance = attackRange * 0.9f;
        }

        State = CombatState.MovingToTarget;
    }

    // =====================================================
    // APPELÉ QUAND ON SE FAIT ATTAQUER
    // =====================================================
    public void OnAttacked(CombatTarget attacker)
    {
        // une attaque subie annule la fuite
        disengageUntilTime = 0f;

        if (attacker == null || !attacker.IsAlive)
            return;

        SetAttackTarget(attacker);
    }

    // =====================================================
    // DÉSENGAGEMENT VOLONTAIRE
    // =====================================================
    public void CancelCombatByPlayer()
    {
        Unit unit = GetComponent<Unit>();
        if (unit == null || unit.unitType != UnitType.Player)
            return;

        disengageUntilTime = Time.time + disengageDuration;
        CancelCombatInternal(false);
    }

    public void CancelCombat()
    {
        disengageUntilTime = Time.time + disengageDuration;
        CancelCombatInternal(false);
    }

    // =====================================================
    // SORTIE COMBAT
    // =====================================================
    void CancelCombatInternal(bool hardDisengage)
    {
        combatActive = false;
        currentTarget = null;
        State = CombatState.Idle;

        if (hardDisengage)
        {
            // 🔒 fuite définitive tant que le joueur ne réengage pas
            disengageUntilTime = Time.time + disengageDuration;
        }

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

        Vector3 targetPos = currentTarget.transform.position;

        if (Vector3.Distance(lastTargetPosition, targetPos) > repathDistance)
        {
            agent.SetDestination(targetPos);
            lastTargetPosition = targetPos;
        }

        agent.isStopped = false;
        RotateTowardsTarget();
    }

    // =====================================================
    // ATTAQUE
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
        if (!combatActive || currentTarget == null)
            return;

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