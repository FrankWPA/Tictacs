using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvTest : MonoBehaviour
{

    public Item item;

    private void Update()
    {
        //Add Items
        if (Input.GetKeyDown(KeyCode.A))
        {
            item.AddItem(1);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            item.AddItem(5);
        }

        //Remove Items
        if (Input.GetKeyDown(KeyCode.Z))
        {
            item.RemoveItem(1);
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            item.RemoveItem(5);
        }

        //Show Inventory
        if (Input.GetKeyDown(KeyCode.I))
        {
            Inventory.ListItems();
        }

    }
}