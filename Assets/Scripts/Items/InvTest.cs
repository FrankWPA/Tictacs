using UnityEngine;

public class InvTest : MonoBehaviour
{

    public Item item;

    public void AddOne ()
    {
        item.AddItem(1);
    }

    public void AddTen()
    {
        item.AddItem(10);
    }

    public void RemoveOne()
    {
        item.RemoveItem(1);
    }

    public void RemoveTen()
    {
        item.RemoveItem(10);
    }
}