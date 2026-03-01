using UnityEngine;

public class CombatTarget : MonoBehaviour
{
    private Health health;

    public bool IsAlive => health != null && !health.isDead;

    void Awake()
    {
        health = GetComponent<Health>();
        if (health == null)
        {
            Debug.LogError($"[CombatTarget] Aucun Health trouvé sur {name}");
        }
    }

    // =====================================================
    // DÉGÂTS AVEC ATTAQUANT
    // =====================================================
    public void TakeDamage(float amount, GameObject attacker)
    {
        if (health == null || health.isDead)
            return;

        health.TakeDamage(amount, attacker);
    }

    // =====================================================
    // DÉGÂTS SANS ATTAQUANT (ENVIRONNEMENT / DOT)
    // =====================================================
    public void TakeDamage(float amount)
    {
        if (health == null || health.isDead)
            return;

        health.TakeDamage(amount, null);
    }
}