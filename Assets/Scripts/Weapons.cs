using UnityEngine;

// =====================================================
// WEAPON (BASE ABSTRAITE)
// =====================================================
public abstract class Weapon : MonoBehaviour
{
    [Header("Weapon Stats")]
    public float baseDamage = 5f;
    public float range = 1.8f;
    public float attackCooldown = 1.2f;

    protected WeaponHandler owner;

    // =====================================================
    // LIFECYCLE
    // =====================================================
    public virtual void OnEquipped(WeaponHandler handler)
    {
        owner = handler;
    }

    public virtual void OnUnequipped()
    {
        owner = null;
    }

    // =====================================================
    // FUTUR : attaque
    // =====================================================
    public abstract void Attack(CombatUnit attacker, CombatUnit target);
}