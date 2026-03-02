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

    [Header("Death")]
    public float destroyDelay = 0f;

    void Awake()
    {
        currentHealth = maxHealth;

        myUnit = GetComponent<Unit>();
        autoDefense = GetComponent<AutoDefense>();
        animator = GetComponentInChildren<Animator>();
        agent = GetComponent<NavMeshAgent>();
        combat = GetComponent<CombatController>();
    }

    void Start()
    {
        // 🔔 Init HUD garanti (Start > Awake UI)
        NotifyHealthChanged();
    }

    // =====================================================
    // TAKE DAMAGE (AVEC OU SANS ATTAQUANT)
    // =====================================================
    public void TakeDamage(float amount, GameObject attacker)
    {
        if (isDead)
            return;

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        NotifyHealthChanged();

        // 🔥 AUTO-DÉFENSE (seulement si attaquant ennemi)
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

        // 🔒 stop combat
        if (combat != null)
            combat.CancelCombat();

        // 🔒 stop navigation
        if (agent != null && agent.enabled)
        {
            agent.isStopped = true;
            agent.enabled = false;
        }

        // 🔒 stop auto-defense
        if (autoDefense != null)
            autoDefense.enabled = false;

        // 🎞️ animation de mort
        if (animator != null)
        {
            animator.ResetTrigger("Attack");
            animator.SetTrigger("Die");
        }

        if (destroyDelay > 0f)
            Destroy(gameObject, destroyDelay);
    }

    // =====================================================
    // HUD SAFE NOTIFY
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