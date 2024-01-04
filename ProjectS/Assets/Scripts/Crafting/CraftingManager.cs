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

    public Transform toolTip { get; private set; }
    public CraftingRecipe currentRecipe { get; private set; }

    [SerializeField] private GameObject requirementPrefab;


    private void Start()
    {
        instance = this;
        toolTip = transform.GetChild(0);
        SetButtonsFunctionality();
    }

    void SetButtonsFunctionality()
    {
        for (int i = 1; i < transform.childCount; i++)
        {
            CraftingRecipe recipe = transform.GetChild(i).GetChild(0).GetComponent<CraftingRecipe>();
            recipe.GetComponent<Button>().onClick.AddListener(() => SetToolTip(recipe));
        }
    }

    void SetTooltipPosition()
    {
        toolTip.transform.position = new Vector2(toolTip.parent.GetComponent<RectTransform>().sizeDelta.x * 4 / 5
                                                , currentRecipe.transform.position.y);
    
    }
    
    void SetTooltipRequirements()
    {
        GameObject itemName = toolTip.GetChild(0).gameObject;
        itemName.GetComponent<TextMeshProUGUI>().text = currentRecipe.prefabItem.name;

        int nrOfReq = currentRecipe.requirements.Length - 1;
        if (nrOfReq > 0)
            toolTip.GetComponent<RectTransform>().sizeDelta = new Vector2(175 + 55 * nrOfReq, toolTip.GetComponent<RectTransform>().sizeDelta.y);
        else
            toolTip.GetComponent<RectTransform>().sizeDelta = new Vector2(175, toolTip.GetComponent<RectTransform>().sizeDelta.y);
    
        int reqInToolTip = toolTip.GetChild(2).childCount;

        if (reqInToolTip < currentRecipe.requirements.Length)
            AddRequirmentsToToolTip(reqInToolTip);
        else
            RemoveRequirementsFromToolTip(reqInToolTip);


    }

    public void SetTooltipCraftButton()
    {
        if (!currentRecipe)
            return;

        if (!toolTip.gameObject.activeInHierarchy)
            return;

        GameObject craftButton = toolTip.GetChild(1).gameObject;
        craftButton.GetComponent<Button>().interactable = currentRecipe.CheckIfCanBeCrafted();
        DisplayRequirements();

        craftButton.GetComponent<Button>().onClick.RemoveAllListeners();
        craftButton.GetComponent<Button>().onClick.AddListener(() => StartCoroutine(CraftRecipe()));
    }


    void SetToolTip(CraftingRecipe recipe)
    {
        if(currentRecipe == recipe)
            toolTip.gameObject.SetActive(!toolTip.gameObject.activeInHierarchy);
        else
            toolTip.gameObject.SetActive(true);

        currentRecipe = recipe;
        SetTooltipPosition();
        SetTooltipRequirements();
        SetTooltipCraftButton();


    }

    IEnumerator CraftRecipe()
    {
        Debug.Log("x");
        GameObject craftedItem = Instantiate(currentRecipe.GetComponent<CraftingRecipe>().prefabItem);

        if (currentRecipe.GetComponent<CraftingRecipe>().isLearned == false) 
            currentRecipe.GetComponent<CraftingRecipe>().isLearned = true;

        InventoryManager.instance.SetBackToSlot();

        if (!craftedItem.GetComponent<Construction>()) // If the crafted thing is not a construction
        {
            int i;
            for ( i = 0; i < currentRecipe.requirements.Length; i++)
                InventoryManager.instance.SpendResources(currentRecipe.requirements[i].type, currentRecipe.requirements[i].quantity);


            yield return null;
            if (craftedItem.GetComponent<Equipment>())
            {
                craftedItem.GetComponent<Item>().SetType(currentRecipe.GetComponent<CraftingRecipe>().prefabItem.name);
                craftedItem.GetComponent<Equipment>().SetDurability(-1);
                InventoryManager.instance.AddItemToSlot(craftedItem);
            }
        }
        else // If the crafted thing is a construction
            PlayerActionManagement.instance.SetTargetAndAction(null, PlayerActionManagement.Action.place); // Set player action to placement mode

        toolTip.gameObject.SetActive(false); // Close tool tip
    }

    void AddRequirmentsToToolTip(int nrOfReqInToolTip)
    {
        for (int i = nrOfReqInToolTip;i< currentRecipe.requirements.Length; i++)
            Instantiate(requirementPrefab).transform.SetParent(toolTip.transform.GetChild(2));

    }

    void RemoveRequirementsFromToolTip(int nrOfReqInToolTip)
    {
        for (int i = currentRecipe.requirements.Length; i < nrOfReqInToolTip; i++)
            Destroy(toolTip.transform.GetChild(2).GetChild(i).gameObject);

    }

    void DisplayRequirements()
    {
        for (int i= 0;i < currentRecipe.requirements.Length; i++)
        {
            Transform req = toolTip.transform.GetChild(2).GetChild(i);
            req.GetChild(0).GetComponent<TextMeshProUGUI>().text = InventoryManager.instance.AmountOwnedOfType(currentRecipe.requirements[i].type) +" / " +currentRecipe.requirements[i].quantity.ToString();

            req.GetComponent<Image>().sprite = ItemsManager.instance.SearchItemsList(currentRecipe.requirements[i].type).GetComponent<Item>().uiImg;
        }

    }

    public void ActivateCraftingButtons(bool mode)
    {
        for (int i = 1; i < transform.childCount; i++)
            transform.GetChild(i).GetChild(0).GetComponent<Button>().interactable = mode;
    }

}
