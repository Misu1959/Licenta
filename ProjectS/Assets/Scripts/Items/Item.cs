using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;

public class Item : MonoBehaviour,IPointerDownHandler
{
    public Sprite uiImg;

    public string   type { get; private set; }
    
    public int      maxStack;
    public int      currentStack { get; private set;}
    
    public int      fuelValue;

    void Start()
    {
        if(currentStack== 0) 
            currentStack = 1;
    }

    public virtual void OnPointerDown(PointerEventData eventData)
    {
        if (Input.GetMouseButtonDown(0))
            PlayerActionManagement.instance.PerformAction(this.gameObject, PlayerActionManagement.Action.pick);

    }

    public virtual void OnMouseEnter()
    {
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

    public void SetTransparent(bool setTransparent)
    {
        Color thisColor = GetComponent<SpriteRenderer>().color;

        if (setTransparent)
            GetComponent<SpriteRenderer>().color = new Color(thisColor.r, thisColor.g, thisColor.b, .5f);
        else
            GetComponent<SpriteRenderer>().color = new Color(thisColor.r, thisColor.g, thisColor.b, 1f);
    }

    public virtual void CreateItemUI(Transform slot,int amount,bool isSelected = false)
    {

        GameObject itemUI = Instantiate(ItemsManager.instance.itemUI);
        AddFunctionalityToItem(itemUI);

        itemUI.GetComponent<Item>().uiImg = uiImg;
        itemUI.GetComponent<Item>().SetType(type);
        itemUI.GetComponent<Item>().currentStack = amount;
        itemUI.GetComponent<Item>().maxStack     = maxStack;
        itemUI.GetComponent<Item>().fuelValue    = fuelValue;
    
        itemUI.gameObject.transform.SetParent(slot);


        if (!isSelected)
            itemUI.gameObject.transform.localPosition = Vector2.zero;
        else
        {
            InventoryManager.instance.selectedItem = itemUI.GetComponent<Item>();
            itemUI.GetComponent<Image>().raycastTarget = false;
        }

        itemUI.gameObject.GetComponent<Image>().sprite = uiImg;
        

    }

    public bool CheckIfStackIsFull()
    {
        if (GetComponent<Equipment>())
            return true;
        if(currentStack == maxStack)
            return true;

        return false;
    }

    void AddFunctionalityToItem(GameObject itemUI)
    {

        if (GetComponent<Food>())
        {
            itemUI.AddComponent<FoodUI>();
            itemUI.GetComponent<Food>().hungerAmount = GetComponent<Food>().hungerAmount;
            itemUI.GetComponent<Food>().hpAmount = GetComponent<Food>().hpAmount;
            return;
        }

        if(GetComponent<Equipment>())
        {
            itemUI.AddComponent<EquipmentUI>();

            itemUI.GetComponent<Equipment>().actionNumber = GetComponent<Equipment>().actionNumber;
            itemUI.GetComponent<Equipment>().SetDurability(GetComponent<Equipment>().durability);
            return;
        }

        itemUI.AddComponent<ItemUI>();

    }

    public void TakeFromStack(int _amountToTake)
    {
        currentStack -= _amountToTake;

        if (currentStack <= 0)
            Destroy(this.gameObject);

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

}