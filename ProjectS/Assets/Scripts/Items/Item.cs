using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;
using static UnityEditor.Progress;

public class Item : MonoBehaviour,IPointerDownHandler
{
    public Sprite uiImg;

    public string   type { get; private set; }
    
    public int      maxStack;
    public int      currentStack { get; private set;}
    
    public int      fuelValue;

    void Start()
    {
        //if(currentStack== 0) 
            //currentStack = 1;
    }

    public virtual void OnPointerDown(PointerEventData eventData)
    {
        if (!InteractionManager.canInteract || InventoryManager.instance.selectedItem)
            return;


        if (Input.GetMouseButtonDown(0))
            if(IsOnTheGround())
        PlayerActionManagement.instance.SetTargetAndAction(this.gameObject, PlayerActionManagement.Action.pick);

    }

    public virtual void OnMouseOver()
    {
        if (!InteractionManager.canInteract || InventoryManager.instance.selectedItem)
        {
            PopUpManager.instance.ShowMousePopUp();
            return;
        }

        if (IsOnTheGround())
            PopUpManager.instance.ShowMousePopUp("LMB - Pick");
    }

    private void OnMouseExit()
    {
        PopUpManager.instance.ShowMousePopUp();
    }

    public void SetType(string _type)
    {
        type = _type;
    }

    public void Drop(Vector2 dropPosition)
    {
        transform.localPosition = dropPosition;
        transform.SetParent(SaveLoadManager.instance.items.transform);

        SetTransparent(true);
        PlayerActionManagement.instance.SetTargetAndAction(this.gameObject, PlayerActionManagement.Action.drop);
        CraftingManager.instance.SetTooltipCraftButton();
    }


    public void SetTransparent(bool setTransparent)
    {
        Color thisColor = GetComponent<SpriteRenderer>().color;

        if (setTransparent)
            GetComponent<SpriteRenderer>().color = new Color(thisColor.r, thisColor.g, thisColor.b, .5f);
        else
            GetComponent<SpriteRenderer>().color = new Color(thisColor.r, thisColor.g, thisColor.b, 1f);
    }

    public virtual Item CreateItemUI(int amount = -1)
    {

        GameObject itemUI = Instantiate(ItemsManager.instance.itemUI);
        AddFunctionalityToItem(itemUI);

        itemUI.GetComponent<Item>().uiImg = uiImg;
        itemUI.GetComponent<Item>().SetType(type);
        itemUI.GetComponent<Item>().currentStack = (amount == -1) ? currentStack : amount;
        itemUI.GetComponent<Item>().maxStack     = maxStack;
        itemUI.GetComponent<Item>().fuelValue    = fuelValue;
        itemUI.gameObject.GetComponent<Image>().sprite = uiImg;

        return itemUI.GetComponent<Item>();
    }

    void AddFunctionalityToItem(GameObject itemUI)
    {

        if (GetComponent<Food>())
        {
            itemUI.AddComponent<FoodUI>();
            itemUI.GetComponent<Food>().hungerAmount = GetComponent<Food>().hungerAmount;
            itemUI.GetComponent<Food>().hpAmount = GetComponent<Food>().hpAmount;
            itemUI.GetComponent<Food>().timeToCook = GetComponent<Food>().timeToCook;

            return;
        }

        if(GetComponent<Equipment>())
        {
            itemUI.AddComponent<EquipmentUI>();

            itemUI.GetComponent<Equipment>().equipmentType = GetComponent<Equipment>().equipmentType;
            itemUI.GetComponent<Equipment>().actionType = GetComponent<Equipment>().actionType;
            itemUI.GetComponent<Equipment>().SetDurability(GetComponent<Equipment>().durability);
            return;
        }

        itemUI.AddComponent<ItemUI>();

    }

    public bool CheckIfStackIsFull()
    {
        if (GetComponent<Equipment>())
            return true;
        if (currentStack == maxStack)
            return true;

        return false;
    }
    public void TakeFromStack(int _amountToTake)
    {
        currentStack -= _amountToTake;

        if (currentStack <= 0)
        {
            PopUpManager.instance.ShowMousePopUp();
            Destroy(this.gameObject);
        }

        GetComponent<ItemUI>()?.DisplayStack();
        GetComponent<FoodUI>()?.DisplayStack();
        GetComponent<EquipmentUI>()?.DisplayStack();
    }

    public void AddToStack(int _amountToTake)
    {
        currentStack += _amountToTake;

        GetComponent<ItemUI>()?.DisplayStack();
        GetComponent<FoodUI>()?.DisplayStack();
        GetComponent<EquipmentUI>()?.DisplayStack();
    }

    protected bool IsOnTheGround()
    {
        if (PlayerActionManagement.instance.currentTarget == this.gameObject &&
        PlayerActionManagement.instance.currentAction == PlayerActionManagement.Action.drop)
            return false;
        else
            return true;
    }

}