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
    // PARAMÈTRES EXISTANTS (fallback si pas d’arme)
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
    public float disengageDistance = 12f;

    private float disengageUntilTime = 0f;

    // =====================================================
    // INTERNES
    // =====================================================
    private CombatTarget currentTarget;
    private NavMeshAgent agent;
    private Animator animator;
    private Weapon weapon;

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
        weapon = GetComponentInChildren<Weapon>();

        if (agent != null)
        {
            agent.updateRotation = false;
            agent.stoppingDistance = attackRange * 0.9f;
        }
    }

    void Update()
    {
        // 🔥 DÉPLACEMENT VOLONTAIRE → ANNULATION COMBAT
        if (combatActive && IsPlayerMovingVoluntarily())
        {
            CancelCombatByPlayer();
            return;
        }

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

        if (agent == null || !agent.enabled)
        {
            State = CombatState.Idle;
            return;
        }

        float hardDist = Vector3.Distance(
            transform.position,
            currentTarget.transform.position
        );

        // rupture par distance
        if (hardDist > disengageDistance)
        {
            CancelCombatInternal(true);
            return;
        }

        float range = weapon != null ? weapon.GetRange() : attackRange;

        if (hardDist > range)
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
    // DÉTECTION INTENTION DE DÉPLACEMENT
    // =====================================================
    bool IsPlayerMovingVoluntarily()
    {
        Unit unit = GetComponent<Unit>();
        if (unit == null || unit.unitType != UnitType.Player)
            return false;

        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        return Mathf.Abs(h) > 0.1f || Mathf.Abs(v) > 0.1f;
    }

    // =====================================================
    // ORDRE D’ATTAQUE
    // =====================================================
    public void SetAttackTarget(CombatTarget target)
    {
        if (target == null || !target.IsAlive)
            return;

        disengageUntilTime = 0f;

        combatActive = true;
        currentTarget = target;
        lastTargetPosition = Vector3.zero;

        if (agent != null && agent.enabled && agent.isOnNavMesh)
        {
            agent.isStopped = false;

            float range = weapon != null ? weapon.GetRange() : attackRange;
            agent.stoppingDistance = range * 0.9f;
        }

        State = CombatState.MovingToTarget;
    }

    // =====================================================
    // ATTAQUE SUBIE → RIPOSTE
    // =====================================================
    public void OnAttacked(CombatTarget attacker)
    {
        disengageUntilTime = 0f;

        if (attacker == null || !attacker.IsAlive)
            return;

        SetAttackTarget(attacker);
    }

    // =====================================================
    // DÉSENGAGEMENT VOLONTAIRE JOUEUR
    // =====================================================
    public void CancelCombatByPlayer()
    {
        Unit unit = GetComponent<Unit>();
        if (unit == null || unit.unitType != UnitType.Player)
            return;

        disengageUntilTime = Time.time + disengageDuration;
        CancelCombatInternal(false);
    }

    // =====================================================
    // ANNULATION COMBAT GÉNÉRALE
    // =====================================================
    public void CancelCombat()
    {
        disengageUntilTime = Time.time + disengageDuration;
        CancelCombatInternal(false);
    }

    void CancelCombatInternal(bool hardDisengage)
    {
        combatActive = false;
        currentTarget = null;
        State = CombatState.Idle;

        if (hardDisengage)
            disengageUntilTime = Time.time + disengageDuration;

        if (animator != null)
            animator.ResetTrigger("Attack");

        if (agent != null && agent.enabled && agent.isOnNavMesh)
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
        if (agent != null && agent.enabled && agent.isOnNavMesh)
            agent.isStopped = true;

        RotateTowardsTarget();

        float cooldown = weapon != null ? weapon.GetCooldown() : attackCooldown;

        if (Time.time - lastAttackTime < cooldown)
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

        float damage = weapon != null ? weapon.GetDamage() : attackDamage;

        Health health = currentTarget.GetComponent<Health>();
        if (health != null)
            health.TakeDamage(damage, gameObject);
    }
}