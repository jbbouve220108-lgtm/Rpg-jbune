using UnityEngine;

public class Companion : MonoBehaviour
{
    public string companionName;

    public bool isRecruited { get; private set; }
    public bool isFollowing { get; private set; }

    private Transform player;

    // 🔹 AJOUTS MINIMAUX
    private Rigidbody rb;
    private Unit unit;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        // 🔹 Récupération des composants physiques
        rb = GetComponent<Rigidbody>();
        unit = GetComponent<Unit>();

        // 🔒 Sécurité : rigidbody requis
        if (rb != null)
        {
            rb.useGravity = true;
            rb.isKinematic = false;
            rb.freezeRotation = true; // évite qu’il tombe sur le côté
        }
    }

    public void Recruit(string newName)
    {
        isRecruited = true;
        companionName = newName;

        CompanionManager.Instance.Register(this);
    }

    // 🔹 Synchronisation du nom (inchangé)
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

        // 🔹 Direction horizontale uniquement (pas de vol)
        Vector3 targetPosition = new Vector3(
            player.position.x,
            rb.position.y,
            player.position.z
        );

        Vector3 direction = (targetPosition - rb.position).normalized;

        // 🔹 Déplacement PHYSIQUE (respect gravité + collisions)
        rb.MovePosition(
            rb.position + direction * 3f * Time.fixedDeltaTime
        );
    }
}