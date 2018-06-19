using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryDisplay : MonoBehaviour {

    public GameObject itemDisplayText;

	// Use this for initialization
	void Start () {
        Inventory.inventoryDisplay = this;
	}

    private void Awake()
    {
        UpdateInventory();
    }

    public void UpdateInventory ()
    {
        var inv = Inventory.inv;

        int numOfItens = inv.Count;
        int numOfChilds = transform.childCount;

        

        //Ajust children number
        if (numOfItens != numOfChilds)
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

        foreach (Item item in inv.Keys)
        {
            foreach (Transform child in transform)
            {

                if (item == null) Debug.Log("Deu ruim galera");
                InventoryItemText inventoryItemText = child.GetComponent<InventoryItemText>();

                if (inventoryItemText.item == null)
                {
                    inventoryItemText.item = item;
                    break;
                }
            }
        }
    }
}
