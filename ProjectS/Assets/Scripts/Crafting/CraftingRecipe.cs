using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CraftingRecipe : MonoBehaviour
{
    private Recipe recipe;

    public Recipe GetRecipe() { return recipe; }

    public void SetRecipe(Recipe _recipe) 
    { 
        recipe = _recipe;
        transform.GetChild(0).GetComponent<Image>().sprite = recipe.recipeUI;
    }

    public IEnumerator CraftRecipe()
    {
        GameObject craftedItem = Instantiate(recipe.prefabItem);

        if (recipe.isLearned == false)
            recipe.isLearned = true;

        InventoryManager.instance.SetBackToSlot();

        if (!craftedItem.GetComponent<Construction>()) // If the crafted thing is not a construction
        {
            foreach (Recipe.Requiremets req in recipe.requirements)
                InventoryManager.instance.SpendResources(req.name, req.quantity);

            yield return null;
            if (craftedItem.GetComponent<Equipment>())
            {
                //craftedItem.GetComponent<Item>().SetName(recipe.prefabItem.name);
                craftedItem.GetComponent<Equipment>().SetDurability(-1);
                InventoryManager.instance.AddItemToSlot(craftedItem.GetComponent<Item>());
            }
        }
        else // If the crafted thing is a construction
            PlayerActionManagement.instance.SetTargetAndAction(null, PlayerActionManagement.Action.place); // Set player action to placement mode

        CraftingManager.instance.SetRecipesList(); // Close crafting manager
    }

    public bool CheckIfCanBeCrafted()
    {
        // If player it's not near a station and recipe isn't learned
        if (PlayerStats.instance.researchLevel == 0)
            if (!recipe.isLearned)
                return false;

        return CheckIfHaveRosources();
    }

    private bool CheckIfHaveRosources()
    {
        foreach(Recipe.Requiremets req in recipe.requirements)
            if (InventoryManager.instance.AmountOwnedOfType(req.name) < req.quantity)
                return false;

        return true;
    }






}
