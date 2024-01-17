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

    public CraftingRecipe currentRecipe { get; private set; }
    private CraftingTab currentTab;


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


    /*


    public void SetToolTip(CraftingRecipe _recipe)
    {
        bool activeStatus;
        if (currentRecipe != _recipe)
            activeStatus = true;
        else
            activeStatus = !toolTip.gameObject.activeInHierarchy;

        currentRecipe = _recipe;

        toolTip.gameObject.SetActive(activeStatus);
        if (activeStatus)
        {
            SetToolTipPosition(_recipe.GetComponent<RectTransform>());
            SetTooltipRequirements();
            SetTooltipCraftButton();
            DisplayRequirements();
        }

    }

    private void SetToolTipPosition(RectTransform _recipe)
    {

    }

    void SetTooltipRequirements()
    {
        GameObject itemName = toolTip.GetChild(0).gameObject;
        itemName.GetComponent<TextMeshProUGUI>().text = currentRecipe.prefabItem.name;

        int nrOfReq = currentRecipe.requirements.Length - 1;
        if (nrOfReq > 0)
            toolTip.parent.GetComponent<RectTransform>().sizeDelta = new Vector2(175 + 55 * nrOfReq, toolTip.GetComponent<RectTransform>().sizeDelta.y);
        else
            toolTip.parent.GetComponent<RectTransform>().sizeDelta = new Vector2(175, toolTip.GetComponent<RectTransform>().sizeDelta.y);

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

    private void SetTooltipCraftButton()
    {
        Button craftButton = toolTip.GetChild(1).GetComponent<Button>();
        craftButton.interactable = currentRecipe.CheckIfCanBeCrafted();

        craftButton.onClick.RemoveAllListeners();
        craftButton.onClick.AddListener(() => StartCoroutine(currentRecipe.CraftRecipe()));

    }

    private void DisplayToolTipRequirements()
    {
        for (int i = 0; i < currentRecipe.requirements.Length; i++)
        {
            Transform req = toolTip.transform.GetChild(2).GetChild(i);
            req.GetChild(0).GetComponent<TextMeshProUGUI>().text = InventoryManager.instance.AmountOwnedOfType(currentRecipe.requirements[i].type) + " / " + currentRecipe.requirements[i].quantity.ToString();

            req.GetComponent<Image>().sprite = ItemsManager.instance.SearchItemsList(currentRecipe.requirements[i].type).GetComponent<Item>().uiImg;
        }

    }



    */

    public void ActivateCraftingManager(bool mode)
    {
        if (!mode)
            SetRecipesList(); // turn of the list

        for (int i = 1; i < transform.childCount; i++)
            transform.GetChild(i).GetChild(0).GetComponent<Button>().interactable = mode;
    }

}
