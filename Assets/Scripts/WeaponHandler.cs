using UnityEngine;

public class WeaponHandler : MonoBehaviour
{
    [Header("Slots")]
    public Transform handSlot;

    [Header("Weapon")]
    [SerializeField]
    private Weapon currentWeapon;

    // =====================================================
    // 🔹 PROPRIÉTÉ PUBLIQUE (MANQUANTE)
    // =====================================================
    public Weapon CurrentWeapon => currentWeapon;

    // =====================================================
    // EQUIP / UNEQUIP
    // =====================================================
    public void EquipWeapon(Weapon weapon)
    {
        if (weapon == null)
            return;

        UnequipWeapon();

        currentWeapon = weapon;

        weapon.transform.SetParent(handSlot);
        weapon.transform.localPosition = Vector3.zero;
        weapon.transform.localRotation = Quaternion.identity;

        weapon.OnEquipped(this);
    }

    public void UnequipWeapon()
    {
        if (currentWeapon == null)
            return;

        currentWeapon.OnUnequipped();
        currentWeapon.transform.SetParent(null);
        currentWeapon = null;
    }
}