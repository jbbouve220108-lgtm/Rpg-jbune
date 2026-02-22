using UnityEngine;

public enum UnitType
{
    Player,
    Recruitable,
    Companion,
    NPC,
    Enemy
}

public class Unit : MonoBehaviour
{
    [Header("Identity")]
    public string unitName = "Unnamed";
    public UnitType unitType = UnitType.NPC;
}