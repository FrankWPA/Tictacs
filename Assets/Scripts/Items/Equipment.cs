using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Equipment", menuName = "Inventory/Equipment")]
public class Equipment : Item
{
    public List<object[]> StatusTriggerList = new List<object[]>();

    public EquipSlot slot = EquipSlot.none;
    public bool twoHanded = false;

    public int damage = 0;
    public int block = 0;
    public int blockRegen = 0;
    public int armour = 0;
    public int armourPirerce = 0;

    public void OnEquip(Character character)
    {
        character.baseDamage += damage;
        character.currentBlock += block;
        character.blockRegen += blockRegen;
        character.armour += armour;
        character.armourPierce += armourPirerce;
        
        StatusTriggerList.ApplyTrigger(character);
    }

    public void OnUnequip(Character character)
    {
        character.baseDamage -= damage;
        character.currentBlock -= block;
        character.blockRegen -= blockRegen;
        character.armour -= armour;
        character.armourPierce -= armourPirerce;

        StatusTriggerList.RemoveTrigger(character);
    }
}

public enum EquipSlot { head, body, hand, offHand, none }