using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentDisplay : MonoBehaviour
{
    public int numb;

    public GameObject itemDisplayText;
    
    public void UpdateEquipment(bool refresh)
    {
        var inv = TurnManager.CurrentSelected.GetComponent<PlayerEquip>().equipList;

        numb = 0;

        foreach (Equipment equipment in inv.Values)
        {
            if (equipment != null) numb++;
        }

        int numOfItens = numb;
        int numOfChilds = transform.childCount;

        //Ajust children number
        
        if (refresh)
        {
            //Kill all children!
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }

            for (int i = 0; i < numOfItens; i++)
            {
                Instantiate(itemDisplayText, transform);
            }
        }

        foreach (EquipSlot slot in inv.Keys)
        {
            if (inv[slot] != null)
            {
                foreach (Transform child in transform)
                {
                    if (inv[slot] == null) Debug.Log("Deu ruim galera");
                    InventoryItemText inventoryItemText = child.GetComponent<InventoryItemText>();

                    if (inventoryItemText.item == null)
                    {
                        inventoryItemText.displayType = DisplayType.Equipment;
                        inventoryItemText.item = inv[slot];
                        inventoryItemText.finalText = (slot.ToString());
                        break;
                    }
                }
            }
        }
    }
}
