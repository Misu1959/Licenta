using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CraftingRecipe : MonoBehaviour
{
    private Recipe recipe;

    public Recipe GetRecipe() { return recipe; }

    public void SetRecipe(Recipe _recipe) { recipe = _recipe;   }

    public bool CheckIfLearned() { return recipe.isLearned; }
    
    public bool CheckIfCanBeCrafted()
    {
        //if (PlayerStats.instance.researchLevel == 0)
        if (CheckIfLearned())
            return CheckIfHaveRosources();

        return false;

    }

    private bool CheckIfHaveRosources()
    {
        foreach (Recipe.Requiremets req in recipe.requirements)
        {
            bool lookInChest = ItemsManager.instance.GetOriginalConstruction(recipe.objectName) == null ? true : false; // If it's a construction don't use items from chest

            if (InventoryManager.instance.AmountOwned(req.name, lookInChest) < req.quantity)
                return false;
        }
        return true;
    }

    public IEnumerator CraftRecipe()
    {
        if (!CheckIfLearned())
            recipe.isLearned = true;

        InventoryManager.instance.SetBackToSlot();

        if (ItemsManager.instance.GetOriginalConstruction(recipe.objectName)) // If the crafted thing is a construction
        {
            Instantiate(ItemsManager.instance.GetOriginalConstruction(recipe.objectName));
            PlayerBehaviour.instance.SetTargetAndAction(null, PlayerBehaviour.Action.place); // Set player action to placement mode


        }
        else 
        {
            foreach (Recipe.Requiremets req in recipe.requirements)
                InventoryManager.instance.SpendResources(req.name, req.quantity);

            yield return null;

            ItemUI craftedItemUI = ItemsManager.instance.CreateItemUI(ItemsManager.instance.GetOriginalItem(recipe.objectName));
            InventoryManager.instance.AddItemToInventory(craftedItemUI);
        }

        CraftingManager.instance.SetRecipesList(); // Close crafting manager
    }

}
