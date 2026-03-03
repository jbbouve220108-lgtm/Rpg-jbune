using UnityEngine;
using System.Collections.Generic;

// =====================================================
// SYSTEME D'EQUIPEMENT COMPLET (LOGIQUE + VISUEL)
// =====================================================

public class Equipment : MonoBehaviour
{
    // =====================================================
    // STOCKAGE LOGIQUE
    // =====================================================

    private Dictionary<EquipmentSlotType, ItemData> equippedItems = new();

    // =====================================================
    // VISUEL
    // =====================================================

    [Header("Visual Attach Points")]
    public Transform weaponAttachPoint;

    private Dictionary<EquipmentSlotType, GameObject> spawnedVisuals = new();

    // =====================================================
    // VALIDATION
    // =====================================================

    public bool CanEquip(ItemData item)
    {
        if (item == null || !item.isEquipable)
            return false;

        return true;
    }

    // =====================================================
    // EQUIP
    // =====================================================

    public void Equip(ItemData item)
    {
        if (!CanEquip(item))
            return;

        EquipmentSlotType slot = item.slotType;

        // Si déjà équipé → retour au sac
        if (equippedItems.ContainsKey(slot))
        {
            ItemData oldItem = equippedItems[slot];
            Inventory.Instance.AddItem(oldItem);

            RemoveVisual(slot);
        }

        equippedItems[slot] = item;
        Inventory.Instance.RemoveItem(item);

        CreateVisual(slot, item);
    }

    // =====================================================
    // UNEQUIP
    // =====================================================

    public void Unequip(EquipmentSlotType slot)
    {
        if (!equippedItems.ContainsKey(slot))
            return;

        ItemData item = equippedItems[slot];
        Inventory.Instance.AddItem(item);

        RemoveVisual(slot);

        equippedItems.Remove(slot);
    }

    // =====================================================
    // GETTERS
    // =====================================================

    public ItemData GetEquipped(EquipmentSlotType slot)
    {
        if (equippedItems.TryGetValue(slot, out ItemData item))
            return item;

        return null;
    }

    public Dictionary<EquipmentSlotType, ItemData> GetAllEquipped()
    {
        return equippedItems;
    }

    // =====================================================
    // VISUAL CREATION
    // =====================================================

    void CreateVisual(EquipmentSlotType slot, ItemData item)
    {
        if (item.worldPrefab == null)
            return;

        if (slot == EquipmentSlotType.Weapon && weaponAttachPoint != null)
        {
            GameObject obj = Instantiate(item.worldPrefab, weaponAttachPoint);
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localRotation = Quaternion.identity;

            spawnedVisuals[slot] = obj;
        }
    }

    void RemoveVisual(EquipmentSlotType slot)
    {
        if (!spawnedVisuals.ContainsKey(slot))
            return;

        if (spawnedVisuals[slot] != null)
            Destroy(spawnedVisuals[slot]);

        spawnedVisuals.Remove(slot);
    }
}