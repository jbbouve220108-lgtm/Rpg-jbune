using UnityEngine;

public abstract class ItemData : ScriptableObject
{
    public string itemName;
    [TextArea] public string description;
    public Sprite icon;

    // Si c'est un équipement
    public bool isEquipable;
    public EquipmentSlotType slotType;
}