using UnityEngine;

public class CombatUnit : MonoBehaviour
{
    [Header("Combat")]
    public float attackRange = 1.8f;
    public float attackCooldown = 1.2f;
    public float baseDamage = 5f;

    private float lastAttackTime;

    private WeaponHandler weaponHandler;
    private CharacterStats stats;

    void Awake()
    {
        weaponHandler = GetComponent<WeaponHandler>();
        stats = GetComponent<CharacterStats>();
    }

    void Update()
    {
        // Pour l’instant : rien en Update
        // L’attaque sera appelée par un ordre plus tard
    }

    // =====================================================
    // ATTAQUE SIMPLE
    // =====================================================
    public void TryAttack(GameObject target)
    {
        if (target == null)
            return;

        if (Time.time - lastAttackTime < attackCooldown)
            return;

        float distance = Vector3.Distance(transform.position, target.transform.position);
        if (distance > attackRange)
            return;

        Health targetHealth = target.GetComponent<Health>();
        if (targetHealth == null)
            return;

        float damage = ComputeDamage();

        targetHealth.TakeDamage(damage);

        lastAttackTime = Time.time;
    }

    // =====================================================
    // CALCUL DES DÉGÂTS
    // =====================================================
    float ComputeDamage()
    {
        float damage = baseDamage;

        if (weaponHandler != null && weaponHandler.CurrentWeapon != null)
        {
            damage += weaponHandler.CurrentWeapon.baseDamage;
        }

        if (stats != null)
        {
            damage += stats.force.value * 0.5f;
        }

        return damage;
    }
}