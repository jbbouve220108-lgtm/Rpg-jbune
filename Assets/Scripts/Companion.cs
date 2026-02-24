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

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        rb = GetComponent<Rigidbody>();
        unit = GetComponent<Unit>();
        selectable = GetComponent<SelectableUnit>();
        agent = GetComponent<NavMeshAgent>();

        if (rb != null)
        {
            rb.freezeRotation = true;
            rb.useGravity = true;

            if (!isRecruited)
            {
                // 🔒 AVANT RECRUTEMENT :
                // gravité OK, déplacements bloqués
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

        if (agent != null)
        {
            agent.enabled = isRecruited;
        }
    }

    public void Recruit(string newName)
    {
        isRecruited = true;
        companionName = newName;

        // 🔓 APRÈS RECRUTEMENT : liberté totale
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

    void LateUpdate()
    {
        if (!isRecruited || unit == null)
            return;

        if (companionName != unit.unitName)
            companionName = unit.unitName;
    }

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

    void FixedUpdate()
    {
        if (!isRecruited)
            return;

        // 🔥 NavMeshAgent prioritaire
        if (agent != null && agent.enabled &&
            agent.remainingDistance > agent.stoppingDistance)
        {
            isFollowing = false;
            return;
        }

        if (!isFollowing || player == null || rb == null)
            return;

        Vector3 selfXZ = new Vector3(rb.position.x, 0f, rb.position.z);
        Vector3 playerXZ = new Vector3(player.position.x, 0f, player.position.z);

        float dist = Vector3.Distance(selfXZ, playerXZ);
        if (dist <= minFollowDistance)
            return;

        Vector3 dir = (playerXZ - selfXZ).normalized;

        rb.MovePosition(
            rb.position + dir * followSpeed * Time.fixedDeltaTime
        );
    }
}