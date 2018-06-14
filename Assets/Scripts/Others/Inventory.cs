using System.Collections.Generic;
using UnityEngine;

public static class Inventory
{
    public static Dictionary<Item, int> inv = new Dictionary<Item, int>();
    public static InventoryDisplay inventoryDisplay = null;

    public static void AddItem(this Item item, int ammount)
    {
        if (inv.ContainsKey(item))
        {
            inv[item] = inv[item] + ammount > 999 ? 999 : inv[item] + ammount;
            return;
        }

        inv.Add(item, ammount);

        if (inventoryDisplay != null) inventoryDisplay.UpdateInventory();
        else Debug.Log("Inventory Display null!");

        return;
    }

    public static bool RemoveItem(this Item item, int ammount)
    {
        if (inv.ContainsKey(item))
        {
            if (inv[item] >= ammount)
            {
                inv[item] -= ammount;
                return true;
            }
            else return false;
        }

        return false;
    }

    public static bool CheckItem(this Item item, int ammount)
    {
        if (inv.ContainsKey(item))
        {
            if (inv[item] >= ammount)
            {
                return true;
            }
            else return false;
        }

        return false;
    }

    public static void ListItems()
    {
        foreach (Item item in inv.Keys)
        {
            Debug.Log(inv[item] + "x " + item.name);
        }
    }
}

