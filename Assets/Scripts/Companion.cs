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
            rb.useGravity = true;
            rb.isKinematic = false;
            rb.freezeRotation = true;
        }

        if (agent != null)
            agent.enabled = false;
    }

    public void Recruit(string newName)
    {
        isRecruited = true;
        companionName = newName;

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
        isFollowing = true;
        if (agent != null)
            agent.enabled = false;
    }

    public void StopFollow()
    {
        isFollowing = false;
        if (agent != null)
            agent.enabled = true;
    }

    void Update()
    {
        // 🔹 Clic droit RTS quand sélectionné
        if (selectable != null && selectable.isSelected && Input.GetMouseButtonDown(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (!Physics.Raycast(ray, out RaycastHit hit))
                return;

            // 👉 Clic droit sur le joueur = FOLLOW
            if (hit.collider.CompareTag("Player"))
            {
                Follow();
                return;
            }

            // 👉 Sinon : arrêt du follow (ordre classique)
            StopFollow();
        }
    }

    void FixedUpdate()
    {
        if (!isFollowing || player == null || rb == null)
            return;

        Vector3 selfXZ = new Vector3(rb.position.x, 0, rb.position.z);
        Vector3 playerXZ = new Vector3(player.position.x, 0, player.position.z);

        float dist = Vector3.Distance(selfXZ, playerXZ);
        if (dist <= minFollowDistance)
            return;

        Vector3 dir = (playerXZ - selfXZ).normalized;

        rb.MovePosition(
            rb.position + dir * followSpeed * Time.fixedDeltaTime
        );
    }
}