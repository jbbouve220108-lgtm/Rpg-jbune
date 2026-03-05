using UnityEngine;
using UnityEngine.AI;

// =====================================================
// ÉTATS DU COMPAGNON (UI / GAMEPLAY)
// =====================================================
public enum CompanionState
{
    Idle,
    Following,
    Hungry,
    Starving,
    Dying
}

public class Companion : MonoBehaviour
{
    public string companionName;

    public bool isRecruited { get; private set; }
    public bool isFollowing { get; private set; }

    private Transform player;

    private Rigidbody rb;
    private Unit unit;
    private SelectableUnit selectable;
    private NavMeshAgent agent;

    // =====================================================
    // ANIMATION
    // =====================================================
    private Animator animator;

    [Header("Follow Settings")]
    public float followSpeed = 3f;
    public float minFollowDistance = 1.8f;

    [Header("Follow Distance")]
    public float followTargetDistance = 2.5f;
    public float followDeadZone = 0.4f;

    [Header("Interaction")]
    public float interactionDistance = 2.0f;

    // =====================================================
    // ROTATION (AJOUT CONTRÔLÉ)
    // =====================================================
    [Header("Rotation")]
    public float navRotationSpeed = 10f;

    // =====================================================
    // ORDRE TEMPORAIRE
    // =====================================================
    private bool hasTemporaryMoveOrder = false;

    // =====================================================
    // STATE
    // =====================================================
    [Header("State")]
    [SerializeField]
    private CompanionState currentState = CompanionState.Idle;
    public CompanionState CurrentState => currentState;

    // =====================================================
    // UNITY
    // =====================================================
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        rb = GetComponent<Rigidbody>();
        unit = GetComponent<Unit>();
        selectable = GetComponent<SelectableUnit>();
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();

        if (rb != null)
        {
            rb.useGravity = true;
            rb.freezeRotation = true;

            if (!isRecruited)
            {
                rb.isKinematic = false;
                rb.constraints =
                    RigidbodyConstraints.FreezePositionX |
                    RigidbodyConstraints.FreezePositionZ |
                    RigidbodyConstraints.FreezeRotation;
            }
            else
            {
                rb.constraints = RigidbodyConstraints.FreezeRotation;
            }
        }

        Collider col = GetComponent<Collider>();
        if (col != null)
            col.isTrigger = false;

        if (agent != null)
        {
            agent.enabled = isRecruited;
            agent.speed = followSpeed;
            agent.acceleration = followSpeed * 4f;
            agent.angularSpeed = 720f;
            agent.stoppingDistance = followTargetDistance - 0.1f;

            // 🔥 important : rotation manuelle
            agent.updateRotation = false;
        }
    }

    // =====================================================
    // INTERACTION
    // =====================================================
    public bool IsPlayerInInteractionRange()
    {
        if (player == null)
            return false;

        float dist = Vector3.Distance(
            new Vector3(transform.position.x, 0f, transform.position.z),
            new Vector3(player.position.x, 0f, player.position.z)
        );

        return dist <= interactionDistance;
    }

    // =====================================================
    // RECRUITMENT
    // =====================================================
    public void Recruit(string newName)
    {
        isRecruited = true;
        companionName = newName;

        if (unit != null)
        {
            unit.unitType = UnitType.Companion;
            unit.unitName = newName;
        }

        if (rb != null)
            rb.constraints = RigidbodyConstraints.FreezeRotation;

        if (agent != null)
            agent.enabled = true;

        CompanionManager.Instance.Register(this);

        // 🔥 AJOUT POUR LE PARTY SYSTEM
        if (PartyManager.Instance != null && unit != null)
        {
            PartyManager.Instance.AddCompanion(unit);
        }
    }

    // =====================================================
    // FOLLOW (UI)
    // =====================================================
    public void Follow()
    {
        if (!isRecruited)
            return;

        isFollowing = true;
        hasTemporaryMoveOrder = false;
    }

    public void StopFollow()
    {
        isFollowing = false;
        hasTemporaryMoveOrder = false;
    }

    // =====================================================
    // FORMATION
    // =====================================================
    public void OnFormationOrder()
    {
        isFollowing = false;
        hasTemporaryMoveOrder = false;
    }

    // =====================================================
    // STATE SYNC
    // =====================================================
    void LateUpdate()
    {
        if (!isRecruited || unit == null)
            return;

        if (companionName != unit.unitName)
            companionName = unit.unitName;

        if (currentState == CompanionState.Hungry ||
            currentState == CompanionState.Starving ||
            currentState == CompanionState.Dying)
            return;

        if (isFollowing)
            currentState = CompanionState.Following;
        else
            currentState = CompanionState.Idle;
    }

    // =====================================================
    // LOGIQUE DE DÉPLACEMENT
    // =====================================================
    void FixedUpdate()
    {
        if (!isRecruited || agent == null || !agent.enabled || !agent.isOnNavMesh)
            return;

        CombatController combat = GetComponent<CombatController>();

        bool isMoving =
            agent.hasPath &&
            agent.remainingDistance > agent.stoppingDistance;

        // 🔥 ROTATION NAVMESH AJOUTÉE (SANS IMPACT COMBAT)
        if (isMoving &&
            (combat == null ||
             combat.State != CombatController.CombatState.Attacking))
        {
            RotateTowardsVelocity();
        }

        if (isMoving)
        {
            hasTemporaryMoveOrder = true;
            UpdateAnimator();
            return;
        }

        if (hasTemporaryMoveOrder && !agent.hasPath)
            hasTemporaryMoveOrder = false;

        if (!isFollowing || player == null)
        {
            UpdateAnimator();
            return;
        }

        float dist = Vector3.Distance(transform.position, player.position);

        if (Mathf.Abs(dist - followTargetDistance) <= followDeadZone)
        {
            UpdateAnimator();
            return;
        }

        Vector3 dir = (transform.position - player.position).normalized;
        Vector3 followPoint = player.position + dir * followTargetDistance;

        agent.isStopped = false;
        agent.SetDestination(followPoint);

        UpdateAnimator();
    }

    // =====================================================
    // ROTATION (AJOUT)
    // =====================================================
    void RotateTowardsVelocity()
    {
        Vector3 vel = agent.velocity;
        vel.y = 0f;

        if (vel.sqrMagnitude < 0.01f)
            return;

        Quaternion targetRot = Quaternion.LookRotation(vel);
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRot,
            Time.deltaTime * navRotationSpeed
        );
    }

    // =====================================================
    // ANIMATION
    // =====================================================
    void UpdateAnimator()
    {
        if (animator == null || agent == null)
            return;

        animator.SetFloat(
            "Speed",
            agent.velocity.magnitude > 0.1f ? 1f : 0f
        );
    }

    // =====================================================
    // STATE API
    // =====================================================
    public void SetState(CompanionState newState)
    {
        if (currentState == CompanionState.Dying)
            return;

        currentState = newState;
    }

    public void SetDyingState()
    {
        currentState = CompanionState.Dying;
    }
}