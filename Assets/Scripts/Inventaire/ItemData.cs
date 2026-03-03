using UnityEngine;

// =====================================================
// BASE ITEM DATA
// =====================================================

public abstract class ItemData : ScriptableObject
{
    [Header("Informations")]
    public string itemName;

    [TextArea]
    public string description;

    public Sprite icon;

    [Header("Equipement")]
    public bool isEquipable;
    public EquipmentSlotType slotType;

    [Header("Visual 3D (Optionnel)")]
    public GameObject worldPrefab;
}