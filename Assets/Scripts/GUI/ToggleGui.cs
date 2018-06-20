using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleGui : MonoBehaviour {

    public GameObject inventory;
    public GameObject equipment;
    public GameObject combatButtons;

    void Update () {
		if (Input.GetButtonDown("ToggleInv"))
        {
            inventory.SetActive(!inventory.activeSelf);
        }
        if (Input.GetButtonDown("ToggleEquip"))
        {
            equipment.SetActive(!equipment.activeSelf);
            equipment.transform.Find("EquipmentPanel").GetComponent<EquipmentDisplay>().UpdateEquipment(false);
        }
        if (TurnManager.combatInitialized != combatButtons.activeSelf)
        {
            combatButtons.SetActive(!combatButtons.activeSelf);
        }
    }
}
