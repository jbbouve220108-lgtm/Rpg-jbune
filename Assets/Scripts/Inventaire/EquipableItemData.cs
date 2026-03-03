using UnityEngine;

// =====================================================
// ITEM DATA CONCRET POUR OBJETS ÉQUIPABLES
// =====================================================

[CreateAssetMenu(menuName = "Items/Equipable Item")]
public class EquipableItemData : ItemData
{
    // =====================================================
    // INITIALISATION AUTOMATIQUE
    // =====================================================
    void OnEnable()
    {
        isEquipable = true;
    }
}