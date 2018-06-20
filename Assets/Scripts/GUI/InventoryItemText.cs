using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItemText : MonoBehaviour {

    public DisplayType displayType = DisplayType.Inventory;
    public Item item;
    public int stack = 0;
    public string finalText;
    Text text;
    
    void Start() {
        text = GetComponent<Text>();
    }

    void Update() {

        switch (displayType) {
            case DisplayType.Inventory:

                stack = Inventory.inv[item];
                if (item == null) text.text = "";
                else
                {
                    finalText = stack + " x " + item.name;

                    if (stack < 10) finalText = "00" + finalText;
                    else if (stack < 100) finalText = "0" + finalText;

                    text.text = finalText;
                }
                break;

            case DisplayType.Equipment:

                if (item == null) text.text = "";
                else
                {
                    text.text = finalText + ": " + item.name;
                }

                    break;
        }
        
    }
}

public enum DisplayType
{
    Inventory, Equipment
}
