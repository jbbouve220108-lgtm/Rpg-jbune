using UnityEngine;
using UnityEngine.AI;

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

    [Header("Follow Settings")]
    public float followSpeed = 3f;
    public float minFollowDistance = 1.8f;

    [Header("Follow Distance")]
    [Tooltip("Distance idéale de suivi")]
    public float followTargetDistance = 2.5f;

    [Tooltip("Zone morte autour de la distance cible (anti-saccades)")]
    public float followDeadZone = 0.4f;

    [Header("Interaction")]
    [Tooltip("Distance minimale pour interagir avec ce PNJ")]
    public float interactionDistance = 2.0f;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        rb = GetComponent<Rigidbody>();
        unit = GetComponent<Unit>();
        selectable = GetComponent<SelectableUnit>();
        agent = GetComponent<NavMeshAgent>();

        // ================= PHYSIQUE =================
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

        // ================= COLLIDER =================
        Collider col = GetComponent<Collider>();
        if (col != null)
            col.isTrigger = false;

        // ================= NAVMESH =================
        if (agent != null)
        {
            agent.enabled = isRecruited;
            agent.speed = followSpeed;
            agent.acceleration = followSpeed * 4f;
            agent.angularSpeed = 720f;
            agent.stoppingDistance = followTargetDistance - 0.1f;
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

        if (rb != null)
            rb.constraints = RigidbodyConstraints.FreezeRotation;

        if (agent != null)
            agent.enabled = true;

        CompanionManager.Instance.Register(this);
    }

    // =====================================================
    // SYNC NOM
    // =====================================================
    void LateUpdate()
    {
        if (!isRecruited || unit == null)
            return;

        if (companionName != unit.unitName)
            companionName = unit.unitName;
    }

    // =====================================================
    // FOLLOW (appel UI)
    // =====================================================
    public void Follow()
    {
        if (!isRecruited)
            return;

        isFollowing = true;
    }

    public void StopFollow()
    {
        isFollowing = false;
    }

    // =====================================================
    // 🔹 AJOUT MINIMAL : APPELÉ PAR LA FORMATION
    // =====================================================
    public void OnFormationOrder()
    {
        // 👉 Une formation coupe simplement le follow
        isFollowing = false;
    }

    // =====================================================
    // LOGIQUE DE DÉPLACEMENT (INCHANGÉE)
    // =====================================================
    void FixedUpdate()
    {
        if (!isRecruited || agent == null || !agent.enabled || !agent.isOnNavMesh || player == null)
            return;

        // 🔥 1. ORDRE MANUEL / FORMATION ACTIF → PRIORITÉ
        if (agent.hasPath && agent.remainingDistance > agent.stoppingDistance)
        {
            return;
        }

        // 🔹 2. PAS EN FOLLOW → ON NE FAIT RIEN
        if (!isFollowing)
            return;

        // 🔹 3. FOLLOW FLUIDE
        float dist = Vector3.Distance(transform.position, player.position);

        if (Mathf.Abs(dist - followTargetDistance) <= followDeadZone)
            return;

        Vector3 dir = (transform.position - player.position).normalized;
        Vector3 followPoint = player.position + dir * followTargetDistance;

        agent.isStopped = false;
        agent.SetDestination(followPoint);
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, interactionDistance);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, followTargetDistance);

        Gizmos.color = Color.gray;
        Gizmos.DrawWireSphere(transform.position, followTargetDistance + followDeadZone);
        Gizmos.DrawWireSphere(transform.position, followTargetDistance - followDeadZone);
    }
#endif
}