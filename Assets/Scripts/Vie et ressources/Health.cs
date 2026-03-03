using UnityEngine;
using UnityEngine.AI;
using System;

public class Health : MonoBehaviour
{
    [Header("Health")]
    public float maxHealth = 100f;
    public float currentHealth;

    public bool isDead { get; private set; }

    // 🔔 EVENT POUR LA HUD
    public event Action<float, float> OnHealthChanged;

    private Unit myUnit;
    private AutoDefense autoDefense;
    private Animator animator;
    private NavMeshAgent agent;
    private CombatController combat;
    private Rigidbody rb;

    [Header("Death")]
    public float destroyDelay = 5f;
    public bool snapToGroundOnDeath = true;

    void Awake()
    {
        currentHealth = maxHealth;

        myUnit = GetComponent<Unit>();
        autoDefense = GetComponent<AutoDefense>();
        animator = GetComponentInChildren<Animator>();
        agent = GetComponent<NavMeshAgent>();
        combat = GetComponent<CombatController>();
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        NotifyHealthChanged();
    }

    // =====================================================
    // TAKE DAMAGE
    // =====================================================
    public void TakeDamage(float amount, GameObject attacker)
    {
        if (isDead)
            return;

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        NotifyHealthChanged();

        // 🔥 AUTO-DÉFENSE
        if (attacker != null &&
            autoDefense != null &&
            myUnit != null)
        {
            Unit attackerUnit = attacker.GetComponent<Unit>();
            if (attackerUnit != null &&
                attackerUnit.unitType != myUnit.unitType)
            {
                autoDefense.OnAttacked(attacker);
            }
        }

        if (currentHealth <= 0f)
            Die();
    }

    // =====================================================
    // HEAL
    // =====================================================
    public void Heal(float amount)
    {
        if (isDead)
            return;

        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        NotifyHealthChanged();
    }

    // =====================================================
    // DIE
    // =====================================================
    void Die()
    {
        if (isDead)
            return;

        isDead = true;
        NotifyHealthChanged();

        // 🔒 Stop combat
        if (combat != null)
            combat.CancelCombat();

        // 🔒 Stop NavMesh
        if (agent != null)
        {
            if (agent.enabled && agent.isOnNavMesh)
            {
                agent.isStopped = true;
                agent.ResetPath();
            }

            agent.enabled = false;
        }

        // 🔒 Stop auto-defense
        if (autoDefense != null)
            autoDefense.enabled = false;

        // 🎞️ Animation de mort
        if (animator != null)
        {
            animator.ResetTrigger("Attack");
            animator.SetTrigger("Die");
        }

        // 🧊 Freeze physique
        FreezePhysics();

        // 🧱 Snap au sol
        if (snapToGroundOnDeath)
            SnapToGround();

        // 🗑️ Despawn
        if (destroyDelay > 0f)
            Destroy(gameObject, destroyDelay);
    }

    // =====================================================
    // FREEZE PHYSICS
    // =====================================================
    void FreezePhysics()
    {
        // 🔒 Désactiver tous les colliders
        Collider[] colliders = GetComponentsInChildren<Collider>();
        foreach (var col in colliders)
            col.enabled = false;

        // 🔒 Stop Rigidbody
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        // 🔒 Rendre non sélectionnable
        gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
    }

    // =====================================================
    // SNAP TO GROUND
    // =====================================================
    void SnapToGround()
    {
        Ray ray = new Ray(transform.position + Vector3.up, Vector3.down);

        if (Physics.Raycast(ray, out RaycastHit hit, 5f))
        {
            Vector3 pos = transform.position;
            pos.y = hit.point.y;
            transform.position = pos;
        }
    }

    // =====================================================
    // HUD NOTIFY
    // =====================================================
    void NotifyHealthChanged()
    {
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    // =====================================================
    // ANIMATION EVENT
    // =====================================================
    public void OnDeathAnimationFinished()
    {
        Destroy(gameObject);
    }
}