using UnityEngine;
using System;

public class Health : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth;

    public bool isDead { get; private set; }

    private Unit myUnit;
    private AutoDefense autoDefense;

    // =====================================================
    // 🆕 EVENT (AJOUT)
    // =====================================================
    public event Action<float, float> OnHealthChanged;

    void Awake()
    {
        currentHealth = maxHealth;
        myUnit = GetComponent<Unit>();
        autoDefense = GetComponent<AutoDefense>();

        NotifyHealthChanged();
    }

    // =====================================================
    // TAKE DAMAGE (AVEC ATTAQUANT)
    // =====================================================
    public void TakeDamage(float amount, GameObject attacker)
    {
        if (isDead)
            return;

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        // 🔒 notification auto-défense
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

        NotifyHealthChanged();

        if (currentHealth <= 0)
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
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        NotifyHealthChanged();
    }

    // =====================================================
    // DIE
    // =====================================================
    void Die()
    {
        isDead = true;
        NotifyHealthChanged();
        Destroy(gameObject);
    }

    // =====================================================
    // 🆕 NOTIFY
    // =====================================================
    void NotifyHealthChanged()
    {
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }
}