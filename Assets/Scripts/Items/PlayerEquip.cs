using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerEquip : MonoBehaviour
{
    // Temporary
    public List<string> newTrigger;
    EquipmentDisplay equipmentDisplay;

    Character character;

    public Equipment[] toEquip = new Equipment[4];

    public Dictionary<EquipSlot, Equipment> equipList = new Dictionary<EquipSlot, Equipment>();

    public void Start()
    {
        equipmentDisplay = Resources.FindObjectsOfTypeAll<EquipmentDisplay>()[0].GetComponent<EquipmentDisplay>();
        character = GetComponent<Character>();

        foreach (EquipSlot slot in Enum.GetValues(typeof(EquipSlot)))
        {
            if (slot != EquipSlot.none) {
                equipList.Add(slot, null);
            }
        }
    }

    public void Equip(Equipment equipment)
    {
        if (CheckSlot(equipment.slot) == null)
        {
            if (equipment.slot == EquipSlot.none)
            {
                Debug.Log("Trying to equip null item!");
                return;
            }

            if (equipment.slot == EquipSlot.offHand)
            {
                if (equipList[EquipSlot.hand] == null ? false : equipList[EquipSlot.hand].twoHanded)
                    Unequip(EquipSlot.hand);
            }

            if (equipment.slot == EquipSlot.hand)
            {
                if (equipList[EquipSlot.offHand] == null ? false : equipment.twoHanded) Unequip(EquipSlot.offHand);
            }

            ApplyEquip(equipment);
        }
        else
        {
            Unequip(equipment.slot);

            if (equipment.slot == EquipSlot.hand)
            {
                if (equipList[EquipSlot.offHand] == null ? false : equipment.twoHanded) Unequip(EquipSlot.offHand);
            }

            ApplyEquip(equipment);
        }
    }

    void ApplyEquip(Equipment equipment)
    {
        equipList[equipment.slot] = equipment;

        // Temporary
        if (newTrigger.Count > 0)
        {
            equipment.StatusTriggerList.Add(new object[newTrigger.Count]);
            for (int i = 0; i < newTrigger.Count; i++)
            {
                equipment.StatusTriggerList[0][i] = newTrigger[i];
            }
        }
        // Temporary

        equipment.OnEquip(character);
    }

    Equipment CheckSlot (EquipSlot slot)
    {
        return equipList[slot];
    }

    public void Unequip(EquipSlot slot)
    {
        equipList[slot].OnUnequip(character);
        equipList[slot].AddItem(1);
        equipList[slot] = null;
    }

    public void ListEquipment()
    {
        foreach (Equipment equip in equipList.Values)
        {
            if (equip != null) Debug.Log(equip.name + " equipped in slot " + equip.slot);
        }
    }

    public void Update()
    {
        if (GetComponent<TacticsMove>().turn) {
            if (Input.GetKeyDown(KeyCode.U))
            {
                foreach(Equipment equip in toEquip)
                {
                    Equip(equip);
                }
                
                equipmentDisplay.UpdateEquipment(true);
            }

            if (Input.GetKeyDown(KeyCode.Y))
            {
                ListEquipment();
            }
        }
    }
}