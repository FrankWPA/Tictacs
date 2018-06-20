using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleGui : MonoBehaviour {

    public GameObject inventory;
    public GameObject equipment;
    public GameObject combatButtons;

    public bool combatActive = true;

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
        if (combatActive != combatButtons.activeSelf)
        {
            combatButtons.SetActive(!combatButtons.activeSelf);
        }
    }
}
