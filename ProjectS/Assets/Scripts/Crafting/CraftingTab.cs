using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CraftingTab : MonoBehaviour
{
    [SerializeField] private List<Recipe> recipesList;

    public List<Recipe> GetRecipes() { return recipesList; }

}
