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

    public Recipe currentRecipe { get; private set; }
    private CraftingTab currentTab;


    [SerializeField] private GameObject requirementPrefab;
    [SerializeField] public GameObject prefabRecipe;


    private RectTransform recipesListPanel;
    private RectTransform toolTip;


    private void Start()
    {
        instance = this;
        recipesListPanel = transform.GetChild(0).GetComponent<RectTransform>();
        toolTip = recipesListPanel.GetChild(0).GetComponent<RectTransform>();
        AddFunctionalityToCraftingTabs();
    }


    void AddFunctionalityToCraftingTabs()
    {
        for (int i = 1; i < transform.childCount; i++)
        {
            CraftingTab craftingTab = transform.GetChild(i).GetComponent<CraftingTab>();
            craftingTab.GetComponent<Button>().onClick.AddListener(() => SetRecipesList(craftingTab));
        }
    }

    public void SetRecipesList(CraftingTab _currentTab = null)
    {
        if(_currentTab == null)
        {
            recipesListPanel.gameObject.SetActive(false);
            currentTab = null;
            return;
        }

        bool activeStatus;
        if (currentTab != _currentTab)
            activeStatus = true;
        else
            activeStatus = !recipesListPanel.gameObject.activeInHierarchy;

        currentTab = _currentTab;

        recipesListPanel.gameObject.SetActive(activeStatus);
        
        if (activeStatus)
        {
            currentTab.CleanRecipesList(recipesListPanel);
            currentTab.AddRecipesToList(recipesListPanel);

            SetRecipesListPosition();

            toolTip.gameObject.SetActive(false);
        }
    }

    void SetRecipesListPosition()
    {
        RectTransform tab = currentTab.GetComponent<RectTransform>();
        recipesListPanel.position = new Vector2(recipesListPanel.position.x,
                                    tab.position.y + (recipesListPanel.rect.height - tab.rect.height) / 2);
    }

    public void SetToolTip(CraftingRecipe craftingRecipeButton)
    {
        bool activeStatus;
        if (currentRecipe != craftingRecipeButton.GetRecipe())
            activeStatus = true;
        else
            activeStatus = !toolTip.gameObject.activeInHierarchy;

        currentRecipe = craftingRecipeButton.GetRecipe();
        toolTip.gameObject.SetActive(activeStatus);
        if (activeStatus)
        {
            SetToolTipPosition(craftingRecipeButton);
            SetTooltipCraftButton(craftingRecipeButton);

            SetTooltipRequirements();
            DisplayToolTipRequirements();
        }

    }

    private void SetToolTipPosition(CraftingRecipe craftingRecipeButton)
    {
        toolTip.transform.position = new Vector2(craftingRecipeButton.GetComponent<RectTransform>().position.x,
                                                craftingRecipeButton.GetComponent<RectTransform>().position.y - 50);
    }

    private void SetTooltipCraftButton(CraftingRecipe _craftingRecipeButton)
    {
        Button craftButton = toolTip.GetChild(1).GetComponent<Button>();
        craftButton.interactable = _craftingRecipeButton.CheckIfCanBeCrafted();

        craftButton.onClick.RemoveAllListeners();
        craftButton.onClick.AddListener(() => StartCoroutine(_craftingRecipeButton.CraftRecipe()));

    }

    void SetTooltipRequirements()
    {
        GameObject itemName = toolTip.GetChild(0).gameObject;
        itemName.GetComponent<TextMeshProUGUI>().text = currentRecipe.prefabItem.name;

        int nrOfReq = currentRecipe.requirements.Length - 1;
        if (nrOfReq > 0)
            toolTip.sizeDelta = new Vector2(175 + 55 * nrOfReq, toolTip.sizeDelta.y);
        else
            toolTip.sizeDelta = new Vector2(175, toolTip.sizeDelta.y);

        int reqInToolTip = toolTip.GetChild(2).childCount;

        if (reqInToolTip < currentRecipe.requirements.Length)
            AddRequirmentsToToolTip(reqInToolTip);
        else
            RemoveRequirementsFromToolTip(reqInToolTip);


    }

    void AddRequirmentsToToolTip(int nrOfReqInToolTip)
    {
        for (int i = nrOfReqInToolTip; i < currentRecipe.requirements.Length; i++)
            Instantiate(requirementPrefab).transform.SetParent(toolTip.transform.GetChild(2));

    }

    void RemoveRequirementsFromToolTip(int nrOfReqInToolTip)
    {
        for (int i = currentRecipe.requirements.Length; i < nrOfReqInToolTip; i++)
            Destroy(toolTip.transform.GetChild(2).GetChild(i).gameObject);

    }



    private void DisplayToolTipRequirements()
    {
        int i = 0;
        foreach(Recipe.Requiremets req in currentRecipe.requirements)
        {
            Transform reqUI = toolTip.transform.GetChild(2).GetChild(i++);
            reqUI.GetChild(0).GetComponent<TextMeshProUGUI>().text = InventoryManager.instance.AmountOwnedOfType(req.type) + " / " + req.quantity.ToString();

            reqUI.GetComponent<Image>().sprite = ItemsManager.instance.SearchItemsList(req.type).GetComponent<Item>().uiImg;
        }

    }

    public void ActivateCraftingManager(bool mode)
    {
        if (!mode)
            SetRecipesList(); // turn of the list

        for (int i = 1; i < transform.childCount; i++)
            transform.GetChild(i).GetComponent<Button>().interactable = mode;
    }

}
