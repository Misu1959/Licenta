using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingTab : MonoBehaviour
{
    [SerializeField] private List<CraftingRecipe> recipesList;


    public void AddRecipesToList(Transform recipesListPanel)
    {
        Transform recipeListContents = recipesListPanel.Find("RecipesList");

        for (int i = 0; i < recipesList.Count; i++)
        {
            //GameObject go = Instantiate(prefabRecipe);
            //go.transform.SetParent(recipeListContents);
        }
    }

    public void CleanRecipesList(Transform recipesListPanel)
    {
        Transform recipeListContents = recipesListPanel.Find("RecipesList");

        foreach (Transform child in recipeListContents)
            Destroy(child.gameObject);
    }

    private void AddFunctionalityToCraftingRecipe()
    {

    }
}
