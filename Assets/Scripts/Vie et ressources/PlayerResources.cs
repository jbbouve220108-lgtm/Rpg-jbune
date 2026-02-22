using UnityEngine;

public class PlayerResources : MonoBehaviour
{
    public static PlayerResources Instance;

    [Header("Resources")]
    public int gold = 100;
    public int food = 10;

    void Awake()
    {
        Instance = this;
    }

    public bool SpendGold(int amount)
    {
        if (gold < amount)
            return false;

        gold -= amount;
        return true;
    }

    public void AddGold(int amount)
    {
        gold += amount;
    }

    public bool SpendFood(int amount)
    {
        if (food < amount)
            return false;

        food -= amount;
        return true;
    }

    public void AddFood(int amount)
    {
        food += amount;
    }
}