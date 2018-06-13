using System.Collections.Generic;
using System.Linq;

public static class CraftManager
{
    public static Dictionary<Recipe, bool> RecipeList = new Dictionary<Recipe, bool>();

    public static bool AddRecipe(this Recipe recipe)
    {
        if (!RecipeList.ContainsKey(recipe))
        {
            RecipeList.Add(recipe, true);
            return (true);
        }
        return (false);
    }

    public static void UpdateRecipes()
    {
        foreach (Recipe recipe in RecipeList.Keys.ToArray())
        {
            int itensChecked = 0;
            foreach (Item item in recipe.Ingredients.Keys)
            {
                itensChecked++;
                if (!item.CheckItem(recipe.Ingredients[item]))
                {
                    RecipeList[recipe] = false;
                }
                else if (itensChecked == recipe.Ingredients.Count)
                {
                    RecipeList[recipe] = true;
                }
            }
        }
    }

    public static bool Craft(this Recipe recipe)
    {
        if (RecipeList[recipe])
        {
            foreach (Item item in recipe.Ingredients.Keys)
            {
                item.RemoveItem(recipe.Ingredients[item]);
            }

            recipe.Result.AddItem(recipe.Ammount);
            return true;
        }
        else return false;
    }

}

