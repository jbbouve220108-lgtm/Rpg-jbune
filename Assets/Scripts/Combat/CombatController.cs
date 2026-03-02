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
    // AJOUTS DÉFENSIFS (AUCUN COMPORTEMENT NOUVEAU)
    // =====================================================
    [Header("Hit Validation")]
    public float hitAngleTolerance = 70f;   // champ frontal réaliste
    public float hitRangeTolerance = 0.4f;  // marge d’erreur animation

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
        lastTargetPosition = Vector3.zero;

        if (agent != null && agent.enabled)
        {
            agent.isStopped = false;
            agent.stoppingDistance = attackRange * 0.9f;
        }

        State = CombatState.MovingToTarget;
    }

    // =====================================================
    // DÉSENGAGEMENT JOUEUR (EXISTANT)
    // =====================================================
    public void CancelCombatByPlayer()
    {
        Unit unit = GetComponent<Unit>();
        if (unit == null || unit.unitType != UnitType.Player)
            return;

        CancelCombat();
    }

    // =====================================================
    // SORTIE COMBAT
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
    // DÉPLACEMENT
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
    // HIT FRAME (SÉCURISÉ)
    // =====================================================
    public void ApplyDamage()
    {
        if (!HasTarget)
            return;

        // 🔒 VALIDATION DISTANCE
        float dist = Vector3.Distance(
            transform.position,
            currentTarget.transform.position
        );

        if (dist > attackRange + hitRangeTolerance)
            return;

        // 🔒 VALIDATION ANGLE (devant moi)
        Vector3 toTarget =
            (currentTarget.transform.position - transform.position).normalized;
        float angle = Vector3.Angle(transform.forward, toTarget);

        if (angle > hitAngleTolerance)
            return;

        Health health = currentTarget.GetComponent<Health>();
        if (health != null)
            health.TakeDamage(attackDamage, gameObject);
    }
}