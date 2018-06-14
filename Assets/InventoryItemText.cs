using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItemText : MonoBehaviour {

    public Item item;
    public int stack = 0;
    Text text;



    // Use this for initialization
    void Start() {
        text = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update() {

        stack = Inventory.inv[item];

        if (item == null) text.text = "";
        else
        {
            string finalText = "" + stack + " x " + item.name;

            if (stack < 10) finalText = "00" + finalText;
            else if (stack < 100) finalText = "0" + finalText;

            text.text = finalText;
        }
    }
}
