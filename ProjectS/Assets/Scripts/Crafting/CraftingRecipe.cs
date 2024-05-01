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
        foreach(Recipe.Requiremets req in recipe.requirements)
            if (InventoryManager.instance.AmountOwned(req.name, !recipe.prefabItem.GetComponent<Construction>()) < req.quantity)
                return false;

        return true;
    }

    public IEnumerator CraftRecipe()
    {

        if (!CheckIfLearned())
            recipe.isLearned = true;

        InventoryManager.instance.SetBackToSlot();

        if (!recipe.prefabItem.GetComponent<Construction>()) // If the crafted thing is not a construction
        {

            foreach (Recipe.Requiremets req in recipe.requirements)
                InventoryManager.instance.SpendResources(req.name, req.quantity);

            yield return null;

            ItemUI craftedItemUI = ItemsManager.instance.CreateItemUI(recipe.prefabItem.GetComponent<Item>());
            InventoryManager.instance.AddItemToInventory(craftedItemUI);
        }
        else // If the crafted thing is a construction
        {
            Instantiate(recipe.prefabItem);
            PlayerBehaviour.instance.SetTargetAndAction(null, PlayerBehaviour.Action.place); // Set player action to placement mode
        }

        CraftingManager.instance.SetRecipesList(); // Close crafting manager
    }

}
