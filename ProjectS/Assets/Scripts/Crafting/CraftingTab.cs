using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CraftingTab : MonoBehaviour
{
    [SerializeField] private List<Recipe> recipesList;


    public void AddRecipesToList(Transform recipesListPanel)
    {
        Transform recipeListContents = recipesListPanel.Find("RecipesList");

        foreach (Recipe recipe in recipesList)
        {
            GameObject go = Instantiate(CraftingManager.instance.prefabRecipe);
            go.transform.SetParent(recipeListContents);

            if (recipe != null)
            {
                go.GetComponent<CraftingRecipe>().SetRecipe(recipe);

                go.GetComponent<Button>().onClick.RemoveAllListeners();
                go.GetComponent<Button>().onClick.AddListener(() => CraftingManager.instance.SetToolTip(go.GetComponent<CraftingRecipe>()));
            }
        }
    }

    public void CleanRecipesList(Transform recipesListPanel)
    {
        Transform recipeListContents = recipesListPanel.Find("RecipesList");

        foreach (Transform child in recipeListContents)
            Destroy(child.gameObject);
    }

}
