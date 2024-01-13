using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CraftingTab : MonoBehaviour
{
    [SerializeField] private GameObject prefabRecipe;

    [SerializeField] private List<CraftingRecipe> recipesList;



    public void SetListPanel(RectTransform listPanel)
    {
        foreach(Transform obj in listPanel.transform)
            Destroy(obj.gameObject); // Destroy old recipes

        foreach (CraftingRecipe recipe in recipesList)
            AddFunctionalityToRecipe(listPanel, recipe); // Adds new recipes
    }

    public void RefreshRecipesList()
    {
        foreach (CraftingRecipe recipe in recipesList)
            recipe.CheckIfCanBeCrafted();
    }

    private void AddFunctionalityToRecipe(RectTransform listPanel,CraftingRecipe _recipe)
    {
        GameObject r = Instantiate(prefabRecipe);
        r.transform.parent = listPanel.transform;
        //        r.AddComponent<CraftingRecipe>()

        r.GetComponent<Button>().onClick.AddListener(() => CraftingManager.instance.SetToolTip(r.GetComponent<CraftingRecipe>()));

    }
}
