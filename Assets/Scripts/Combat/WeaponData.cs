using UnityEngine;

[CreateAssetMenu(fileName = "NewWeapon", menuName = "Combat/Weapon")]
public class WeaponData : ScriptableObject
{
    [Header("Base Stats")]
    public string weaponName;
    public float damage = 20f;
    public float attackRange = 1.8f;
    public float attackCooldown = 1.2f;

    [Header("Future Extensions")]
    public float critChance = 0f;
    public float critMultiplier = 1.5f;
}