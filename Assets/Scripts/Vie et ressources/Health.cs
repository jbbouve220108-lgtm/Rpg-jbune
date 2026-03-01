using UnityEngine;

public class Health : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth;

    public bool isDead { get; private set; }

    private Unit myUnit;
    private AutoDefense autoDefense;

    void Awake()
    {
        currentHealth = maxHealth;
        myUnit = GetComponent<Unit>();
        autoDefense = GetComponent<AutoDefense>();
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

        // =================================================
        // 🔒 NOTIFICATION AUTO-DÉFENSE (ANTI FRIENDLY FIRE)
        // =================================================
        if (attacker != null &&
            autoDefense != null &&
            myUnit != null)
        {
            Unit attackerUnit = attacker.GetComponent<Unit>();

            // 🔥 SEULEMENT SI L’ATTAQUANT EST UN ENNEMI
            if (attackerUnit != null &&
                attackerUnit.unitType != myUnit.unitType)
            {
                autoDefense.OnAttacked(attacker);
            }
        }

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
    }

    // =====================================================
    // DIE
    // =====================================================
    void Die()
    {
        isDead = true;
        Destroy(gameObject);
    }
}