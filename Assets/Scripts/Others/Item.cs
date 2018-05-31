using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public itemType ItemType = itemType.newItem;
    public int maxStackQuantity = 1;
    public int stackQuantity = 1;

    public Item NewItem(itemType Type)
    {
        switch (Type)
        {
            case itemType.Armour:
                return new Armour
                {
                    ItemType = itemType.Armour
                };
            case itemType.Weapon:
                return new Weapon
                {
                    ItemType = itemType.Weapon
                };
            case itemType.Consumable:
                return new Consumable
                {
                    ItemType = itemType.Consumable
                };
            case itemType.Material:
                return new Material
                {
                    ItemType = itemType.Material
                };
            default: return null;
        }
    }
}

public enum itemType
{
    newItem, Weapon, Armour, Consumable, Material
}

public class Armour : Item
{

}

public class Consumable : Item
{

}

public class Weapon : Item
{

}

public class Material : Item
{

}