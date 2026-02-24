using UnityEngine;

public class Companion : MonoBehaviour
{
    public string companionName;

    public bool isRecruited { get; private set; }
    public bool isFollowing { get; private set; }

    private Transform player;

    // 🔹 Physique
    private Rigidbody rb;
    private Unit unit;

    // 🔹 NOUVEAU : distance minimale de follow
    [Header("Follow Settings")]
    public float followSpeed = 3f;
    public float minFollowDistance = 1.8f;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        rb = GetComponent<Rigidbody>();
        unit = GetComponent<Unit>();

        if (rb != null)
        {
            rb.useGravity = true;
            rb.isKinematic = false;
            rb.freezeRotation = true;
        }
    }

    public void Recruit(string newName)
    {
        isRecruited = true;
        companionName = newName;

        CompanionManager.Instance.Register(this);
    }

    // 🔹 Synchronisation du nom
    void LateUpdate()
    {
        if (!isRecruited || unit == null)
            return;

        if (companionName != unit.unitName)
        {
            companionName = unit.unitName;
        }
    }

    public void Follow()
    {
        isFollowing = true;
    }

    public void StopFollow()
    {
        isFollowing = false;
    }

    void FixedUpdate()
    {
        if (!isFollowing || player == null || rb == null)
            return;

        // 🔹 Positions horizontales (on ignore Y)
        Vector3 companionPos = new Vector3(rb.position.x, 0f, rb.position.z);
        Vector3 playerPos = new Vector3(player.position.x, 0f, player.position.z);

        float distance = Vector3.Distance(companionPos, playerPos);

        // 🔒 Distance minimale atteinte → on ne bouge plus
        if (distance <= minFollowDistance)
            return;

        Vector3 direction = (playerPos - companionPos).normalized;

        rb.MovePosition(
            rb.position + direction * followSpeed * Time.fixedDeltaTime
        );
    }
}