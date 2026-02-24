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
        {
            col.isTrigger = false;
        }

        // ================= NAVMESH =================
        if (agent != null)
        {
            agent.enabled = isRecruited;
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
        {
            rb.constraints = RigidbodyConstraints.FreezeRotation;
        }

        if (agent != null)
        {
            agent.enabled = true;
        }

        CompanionManager.Instance.Register(this);
    }

    // =====================================================
    // FOLLOW (appelé par UICompanions)
    // =====================================================
    public void Follow()
    {
        if (!isRecruited || agent == null || player == null)
            return;

        isFollowing = true;

        if (!agent.enabled || !agent.isOnNavMesh)
            return;

        agent.isStopped = false;
        agent.SetDestination(player.position);
    }

    public void StopFollow()
    {
        isFollowing = false;

        if (agent != null && agent.enabled && agent.isOnNavMesh)
        {
            agent.ResetPath();
        }
    }

    // =====================================================
    // UPDATE LOGIQUE
    // =====================================================
    void FixedUpdate()
    {
        if (!isRecruited || agent == null || !agent.enabled || !agent.isOnNavMesh || player == null)
            return;

        // 🔥 PRIORITÉ À UN ORDRE DE DÉPLACEMENT
        if (agent.hasPath && agent.remainingDistance > agent.stoppingDistance)
            return;

        if (!isFollowing)
            return;

        float dist = Vector3.Distance(transform.position, player.position);
        if (dist <= minFollowDistance)
            return;

        agent.isStopped = false;
        agent.SetDestination(player.position);
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, interactionDistance);
    }
#endif
}