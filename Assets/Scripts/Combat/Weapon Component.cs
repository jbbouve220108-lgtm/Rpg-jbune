using UnityEngine;

public class Weapon : MonoBehaviour
{
    public WeaponData data;

    public float GetDamage()
    {
        if (data == null) return 0f;

        float damage = data.damage;

        // futur crit system
        if (data.critChance > 0f && Random.value < data.critChance)
        {
            damage *= data.critMultiplier;
        }

        return damage;
    }

    public float GetRange()
    {
        return data != null ? data.attackRange : 1.8f;
    }

    public float GetCooldown()
    {
        return data != null ? data.attackCooldown : 1.2f;
    }
}