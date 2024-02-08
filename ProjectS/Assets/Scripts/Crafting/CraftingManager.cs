using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class CraftingManager : MonoBehaviour
{
    public static CraftingManager instance;

    private CraftingRecipe currentCraftingRecipe;
    private CraftingTab currentCraftingTab;


    [SerializeField] private GameObject requirementPrefab;
    [SerializeField] private GameObject prefabRecipe;

    private RectTransform recipesListPanel;
    private RectTransform toolTip;

    private void Start()
    {
        instance = this;
        recipesListPanel = transform.GetChild(0).GetComponent<RectTransform>();
        toolTip = recipesListPanel.GetChild(0).GetComponent<RectTransform>();
        AddFunctionalityToCraftingTabs();
    }

    public CraftingTab GetCurrentCraftingTab() { return currentCraftingTab; }
    public CraftingRecipe GetCurrentCraftingRecipe() { return currentCraftingRecipe; }

    private void AddFunctionalityToCraftingTabs()
    {
        for (int i = 1; i < transform.childCount; i++)
        {
            CraftingTab craftingTab = transform.GetChild(i).GetComponent<CraftingTab>();
            craftingTab.GetComponent<Button>().onClick.AddListener(() => SetRecipesList(craftingTab));
        }
    }

    public void ActivateCraftingManager(bool mode)
    {
        if (!mode)
            SetRecipesList(); // turn of the list

        for (int i = 1; i < transform.childCount; i++)
            transform.GetChild(i).GetComponent<Button>().interactable = mode;
    }

    public IEnumerator RefreshCraftingManager()
    {
        yield return null; // Wait a frame
        if(currentCraftingTab)
            RefreshRecipesList();
        
        if(currentCraftingRecipe)
            RefreshToolTip();
    }


    #region Recipes list

    public void SetRecipesList(CraftingTab newCraftingTab = null)
    {
        if(newCraftingTab == null)
        {
            recipesListPanel.gameObject.SetActive(false);
            currentCraftingTab = null;
            return;
        }

        bool activeStatus;
        if (currentCraftingTab != newCraftingTab)
        {
            activeStatus = true;
            currentCraftingTab = newCraftingTab;
        }
        else
        {
            activeStatus = !recipesListPanel.gameObject.activeInHierarchy;
            currentCraftingTab = null;
        }

        recipesListPanel.gameObject.SetActive(activeStatus);
        if (activeStatus)
        {
            CleanRecipesList();
            AddRecipesToList();

            SetRecipesListPosition();
            DisplayRecipesList();
            toolTip.gameObject.SetActive(false);
        }
    }

    private void SetRecipesListPosition()
    {
        RectTransform tab = currentCraftingTab.GetComponent<RectTransform>();
        recipesListPanel.position = new Vector2(recipesListPanel.position.x,
                                    tab.position.y + (recipesListPanel.rect.height - tab.rect.height) / 2);
    }

    private void AddRecipesToList()
    {
        Transform recipeListContents = recipesListPanel.Find("RecipesList");
        foreach (Recipe recipe in currentCraftingTab.GetRecipes())
        {
            GameObject go = Instantiate(prefabRecipe);
            go.transform.SetParent(recipeListContents);

            if (recipe != null)
            {
                go.GetComponent<CraftingRecipe>().SetRecipe(recipe);

                go.GetComponent<Button>().onClick.RemoveAllListeners();
                go.GetComponent<Button>().onClick.AddListener(() => SetToolTip(go.GetComponent<CraftingRecipe>()));
            }
        }
    }

    private void CleanRecipesList()
    {
        Transform recipeListContents = recipesListPanel.Find("RecipesList");

        foreach (Transform craftingRecipe in recipeListContents)
            Destroy(craftingRecipe.gameObject);
    }

    private void DisplayRecipesList()
    {
        Transform recipeListContents = recipesListPanel.Find("RecipesList");
        foreach (Transform craftingRecipe in recipeListContents)
        {

            if (craftingRecipe.GetComponent<CraftingRecipe>().CheckIfCanBeCrafted())
            {
                craftingRecipe.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
                craftingRecipe.GetChild(0).GetComponent<Image>().color = new Color32(255, 255, 255, 255);
            }
            else
            {
                craftingRecipe.GetComponent<Image>().color = new Color32(255, 255, 255, 128);
                craftingRecipe.GetChild(0).GetComponent<Image>().color = new Color32(255, 255, 255, 128);
            }

            craftingRecipe.GetChild(0).GetComponent<Image>().sprite = craftingRecipe.GetComponent<CraftingRecipe>().GetRecipe().recipeUI;
            craftingRecipe.GetChild(1).gameObject.SetActive(!craftingRecipe.GetComponent<CraftingRecipe>().CheckIfLearned());
        }
    }

    private void RefreshRecipesList() {   DisplayRecipesList();   }

    #endregion


    #region Tooltip
    private void SetToolTip(CraftingRecipe newCraftingRecipe)
    {
        if (currentCraftingRecipe != newCraftingRecipe)
        {
            toolTip.gameObject.SetActive(true);
            currentCraftingRecipe = newCraftingRecipe;

            SetToolTipPosition();

            SetTooltipRequirements();
            SetTooltipCraftButton();

            Invoke(nameof(DisplayToolTipRequirements), .01f);
        }
        else
        {
            toolTip.gameObject.SetActive(false);
            currentCraftingRecipe = null;
        }
    }

    private void SetToolTipPosition()
    {
        toolTip.transform.position = new Vector2(currentCraftingRecipe.GetComponent<RectTransform>().position.x,
                                                currentCraftingRecipe.GetComponent<RectTransform>().position.y - 50);
    }

    private void SetTooltipCraftButton()
    {
        Button craftButton = toolTip.GetChild(1).GetComponent<Button>();
        craftButton.interactable = currentCraftingRecipe.CheckIfCanBeCrafted();

        craftButton.onClick.RemoveAllListeners();
        craftButton.onClick.AddListener(() => StartCoroutine(currentCraftingRecipe.CraftRecipe()));
    }

    private void SetTooltipRequirements()
    {
        TextMeshProUGUI itemName = toolTip.GetChild(0).GetComponent<TextMeshProUGUI>();
        itemName.text = currentCraftingRecipe.GetRecipe().prefabItem.name;

        int nrOfReq = currentCraftingRecipe.GetRecipe().requirements.Length - 1;
        if (nrOfReq > 0)
            toolTip.sizeDelta = new Vector2(175 + 55 * nrOfReq, toolTip.sizeDelta.y);
        else
            toolTip.sizeDelta = new Vector2(175, toolTip.sizeDelta.y);

        RemoveRequirementsFromToolTip();
        AddRequirementsToToolTip();
    }

    private void AddRequirementsToToolTip()
    {
        Transform requirementsList = toolTip.transform.GetChild(2);
        for (int i = 0; i < currentCraftingRecipe.GetRecipe().requirements.Length; i++)
            Instantiate(requirementPrefab).transform.SetParent(requirementsList);
    }

    private void RemoveRequirementsFromToolTip()
    {
        Transform requirementsList = toolTip.transform.GetChild(2);
        foreach (Transform req in requirementsList)
            Destroy(req.gameObject);

    }
    private void DisplayToolTipRequirements()
    {
        Transform requirementsList = toolTip.transform.GetChild(2);
        for(int i = 0;i< currentCraftingRecipe.GetRecipe().requirements.Length; i++) 
        {
            Recipe.Requiremets req = currentCraftingRecipe.GetRecipe().requirements[i];
            Transform reqUI = requirementsList.GetChild(i);

            reqUI.GetComponent<Image>().sprite = ItemsManager.instance.SearchItemsList(req.name).GetItemData().uiImg;
            reqUI.GetChild(0).GetComponent<TextMeshProUGUI>().text = InventoryManager.instance.AmountOwned(req.name) + " / " + req.quantity.ToString();
        }

    }

    private void RefreshToolTip()
    {
        SetTooltipCraftButton();
        DisplayToolTipRequirements();
    }

    #endregion


}
