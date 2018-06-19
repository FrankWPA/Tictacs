using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Recipe : MonoBehaviour
{
    public Dictionary<Item, int> Ingredients = new Dictionary<Item, int>();
    public Item Result;
    public int Ammount;

    public List<Item> newIngredient;
    public List<int> newAmmount;

    public int ingredientAmmount;

    public void Start()
    {
        AddSpace();
    }

    [ContextMenu("Apply Recipe")]
    public void ApplyRecipe()
    {
        this.AddRecipe();
    }

    [ContextMenu("Add Ingredient")]
    public void AddIngredient()
    {
        for (int i = 0; i < newIngredient.Count; i++) {
            Ingredients.Add(newIngredient[i], newAmmount[i]);
            newIngredient[i] = null;
            newAmmount[i] = 0;
        }

    }

    [ContextMenu("Add Space")]
    public void AddSpace()
    {
        for (int i = 0; i < ingredientAmmount; i++)
        {
            newIngredient.Add(null);
            newAmmount.Add(0);
        }

    }
}