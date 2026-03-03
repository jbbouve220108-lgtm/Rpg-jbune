using UnityEngine;
using System.Collections.Generic;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance;

    // Stack infini
    private Dictionary<ItemData, int> items = new();

    void Awake()
    {
        Instance = this;
    }

    public void AddItem(ItemData item, int amount = 1)
    {
        if (!items.ContainsKey(item))
            items[item] = 0;

        items[item] += amount;
    }

    public void RemoveItem(ItemData item, int amount = 1)
    {
        if (!items.ContainsKey(item))
            return;

        items[item] -= amount;

        if (items[item] <= 0)
            items.Remove(item);
    }

    public int GetAmount(ItemData item)
    {
        if (items.TryGetValue(item, out int amount))
            return amount;

        return 0;
    }

    public Dictionary<ItemData, int> GetAllItems()
    {
        return items;
    }
}