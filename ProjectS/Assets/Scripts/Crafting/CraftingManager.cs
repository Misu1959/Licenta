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

    public GameObject toolTip { get; private set; }
    public GameObject currentRecipe { get; private set; }

    [SerializeField] private GameObject requirementPrefab;


    private void Start()
    {
        instance = this;
        toolTip = transform.GetChild(0).gameObject;
        SetButtonsFunctionality();
    }

    void SetButtonsFunctionality()
    {
        for (int i = 1; i < transform.childCount; i++)
        {
            GameObject recipe = transform.GetChild(i).GetChild(0).gameObject;
            recipe.GetComponent<Button>().onClick.AddListener(() => SetToolTip(recipe));
        }
    }

    void SetTooltipPosition()
    {
        toolTip.transform.position = new Vector2(toolTip.transform.parent.GetComponent<RectTransform>().sizeDelta.x * 4 / 5
                                                , currentRecipe.transform.position.y);
    
    }
    
    void SetTooltipRequirements()
    {
        GameObject itemName = toolTip.transform.GetChild(0).gameObject;
        itemName.GetComponent<TextMeshProUGUI>().text = currentRecipe.GetComponent<CraftingRecipe>().prefabItem.name;

        int nrOfReq = currentRecipe.GetComponent<CraftingRecipe>().requirements.Length - 1;
        if (nrOfReq > 0)
            toolTip.GetComponent<RectTransform>().sizeDelta = new Vector2(175 + 55 * nrOfReq, toolTip.GetComponent<RectTransform>().sizeDelta.y);
        else
            toolTip.GetComponent<RectTransform>().sizeDelta = new Vector2(175, toolTip.GetComponent<RectTransform>().sizeDelta.y);
    
        int reqInToolTip = toolTip.transform.GetChild(2).childCount;

        if (reqInToolTip < currentRecipe.GetComponent<CraftingRecipe>().requirements.Length)
            AddRequirmentsToToolTip(reqInToolTip);
        else
            RemoveRequirementsFromToolTip(reqInToolTip);


    }

    public void SetTooltipCraftButton()
    {
        if (!currentRecipe)
            return;

        if (!toolTip.activeInHierarchy)
            return;

        GameObject craftButton = toolTip.transform.GetChild(1).gameObject;
        craftButton.GetComponent<Button>().interactable = currentRecipe.GetComponent<CraftingRecipe>().CheckIfCanBeCrafted();
        DisplayRequirements();

        craftButton.GetComponent<Button>().onClick.RemoveAllListeners();
        craftButton.GetComponent<Button>().onClick.AddListener(() => CraftRecipe());
    }


    void SetToolTip(GameObject recipe)
    {
        if(currentRecipe == recipe)
            toolTip.SetActive(!toolTip.activeInHierarchy);
        else
            toolTip.SetActive(true);

        currentRecipe = recipe;
        SetTooltipPosition();
        SetTooltipRequirements();
        SetTooltipCraftButton();


    }

    void CraftRecipe()
    {
        GameObject craftedItem = Instantiate(currentRecipe.GetComponent<CraftingRecipe>().prefabItem);

        if (currentRecipe.GetComponent<CraftingRecipe>().isLearned == false)
            currentRecipe.GetComponent<CraftingRecipe>().isLearned = true;

        InventoryManager.instance.SetBackToSlot();

        if (craftedItem.GetComponent<Equipment>())
        {
            craftedItem.GetComponent<Item>().SetType(currentRecipe.GetComponent<CraftingRecipe>().prefabItem.name);
            craftedItem.GetComponent<Equipment>().SetDurability(-1);
            InventoryManager.instance.AddItemToSlot(craftedItem);
        }

        if (!currentRecipe.GetComponent<CraftingRecipe>().prefabItem.GetComponent<Construction>())
        {
            for (int i = 0; i < currentRecipe.GetComponent<CraftingRecipe>().requirements.Length; i++)
                InventoryManager.instance.SpendResources(currentRecipe.GetComponent<CraftingRecipe>().requirements[i].type, currentRecipe.GetComponent<CraftingRecipe>().requirements[i].quantity);
        }
        else
            PlayerActionManagement.instance.SetTargetAndAction(null, PlayerActionManagement.Action.place);

        toolTip.SetActive(false);
    }

    void AddRequirmentsToToolTip(int nrOfReqInToolTip)
    {
        for (int i = nrOfReqInToolTip;i< currentRecipe.GetComponent<CraftingRecipe>().requirements.Length; i++)
            Instantiate(requirementPrefab).transform.SetParent(toolTip.transform.GetChild(2));

    }

    void RemoveRequirementsFromToolTip(int nrOfReqInToolTip)
    {
        for (int i = currentRecipe.GetComponent<CraftingRecipe>().requirements.Length; i < nrOfReqInToolTip; i++)
            Destroy(toolTip.transform.GetChild(2).GetChild(i).gameObject);

    }

    void DisplayRequirements()
    {
        for (int i= 0;i < currentRecipe.GetComponent<CraftingRecipe>().requirements.Length; i++)
        {
            Transform req = toolTip.transform.GetChild(2).GetChild(i);
            req.GetChild(0).GetComponent<TextMeshProUGUI>().text = InventoryManager.instance.AmountOwnedOfType(currentRecipe.GetComponent<CraftingRecipe>().requirements[i].type) +" / " +currentRecipe.GetComponent<CraftingRecipe>().requirements[i].quantity.ToString();

            req.GetComponent<Image>().sprite = ItemsManager.instance.SearchItemsList(currentRecipe.GetComponent<CraftingRecipe>().requirements[i].type).GetComponent<Item>().uiImg;
        }

    }

    public void ActivateCraftingButtons(bool mode)
    {
        for (int i = 1; i < transform.childCount; i++)
            transform.GetChild(i).GetChild(0).GetComponent<Button>().interactable = mode;
    }

}
