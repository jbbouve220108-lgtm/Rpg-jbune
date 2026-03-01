using UnityEngine;

public class Health : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth;

    public bool isDead { get; private set; }

    void Awake()
    {
        currentHealth = maxHealth;
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

        // 🔥 AUTO-DÉFENSE
        AutoDefense defense = GetComponent<AutoDefense>();
        if (defense != null && attacker != null)
        {
            defense.OnAttacked(attacker);
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