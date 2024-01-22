using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance;

    public Transform inventory;
    private InventorySlot[] slots = new InventorySlot[15];
    
    public Item selectedItem { get; private set; }

    void Start()
    {
        instance = this;
    
        SetSlots();
    }

    void SetSlots()
    {
        for (int i = 0; i < slots.Length; i++)
            slots[i] = inventory.GetChild(i).GetComponent<InventorySlot>();
    }

    private InventorySlot FindFreeSlot()
    {
        foreach (InventorySlot slot in slots)
        {
            if (!slot.CheckIfItHasItem()) // If the slot is empty
                return slot; // Return an empty slot
        }

        return null;
    }
    public InventorySlot FindSlot(Item itemToAdd)
    {
        foreach (InventorySlot slot in slots)
        {
            if (slot.CheckIfItHasItem())   // Check if there is an item in slot
                if (slot.CheckMatchingName(itemToAdd.name)) // Check if the items have the same type
                    if(!slot.GetItemInSlot().CheckIfStackIsFull())  // If the item in slot does't have a full stack return it
                        return slot;

        }

        return FindFreeSlot(); // If we haven't found a slot that is partialy full return an empty slot if this exist
    }

    public void AddItemToSlot(Item itemToAdd)
    {
        InventorySlot slot = FindSlot(itemToAdd);

        if (slot == null) // If I haven't found an available slot
        {
            // I drop the item on the floor
            itemToAdd.Drop(PlayerStats.instance.transform.position);
        }
        else if (!slot.CheckIfItHasItem()) // if the slot is empty
        {
            slot.SetItemInSlot(itemToAdd.CreateItemUI(itemToAdd.currentStack));// Create the ItemUi and add it to the free slot
            Destroy(itemToAdd.gameObject); //Destroy the item
            
            //CraftingManager.instance.Set();
        }
        else if(slot.CheckIfItHasItem())// If the slot is partially full
        {
            Item itemInSlot = slot.GetItemInSlot(); // I get the item that is in slot
            int dif = itemInSlot.maxStack- itemInSlot.currentStack; // I calculate how much can I add to the item stack

            if (itemToAdd.currentStack < dif) // I check If I can't add both's item's stack together in one
            {
                itemInSlot.AddToStack(itemToAdd.currentStack);
                Destroy(itemToAdd.gameObject);
            }
            else // If not I add them until the item that is in stock has a full stack
            {
                itemToAdd.TakeFromStack(dif);
                itemInSlot.AddToStack(dif);

                AddItemToSlot(itemToAdd); // I call the function again so that I look for a new slot
            }
            //CraftingManager.instance.SetTooltipCraftButton();
        }
        

    }

    public void SwapTwoSlots(InventorySlot _slot)
    {
        Item aux = _slot.GetItemInSlot();
        _slot.SetItemInSlot(selectedItem);
        
        SetSelectedItem(aux);
    }

    public void SetBackToSlot(Item item = null)
    {
        // If itemToSetBackInInventory is null it means we want to sand back selected item
        Item itemToSetInInventory;
        if (item)
            itemToSetInInventory = item;
        else
            itemToSetInInventory = selectedItem;


        if (!itemToSetInInventory)  // if the item is null return
            return;
        
        FindFreeSlot().SetItemInSlot(selectedItem); // Set the item to a free slot
    }

    public void SetSelectedItem(Item itemToSelect)
    {
        selectedItem = itemToSelect; // Set the selected item
        selectedItem?.transform.SetParent(inventory.parent); // Set it's parent

        // If player is going to add fuel or to cook and I change the selected item or unselect it cancel the action
        if(PlayerActionManagement.instance.currentAction == PlayerActionManagement.Action.addFuel ||
            PlayerActionManagement.instance.currentAction == PlayerActionManagement.Action.cook)
            PlayerActionManagement.instance.CancelAction();
    }

    public bool CheckSelecteditem(Item itemToCheck)
    {
        return (selectedItem != itemToCheck) ? false : true;
    }

    public int AmountOwnedOfType(Item.Name _name)
    {
        int totalAmount = 0;

        foreach (InventorySlot slot in slots)
            if (slot.CheckIfItHasItem())
                if (slot.CheckMatchingName(_name))
                    totalAmount += slot.GetItemInSlot().currentStack;

        return totalAmount;
    }

    public void SpendResources(Item.Name _name, int _amount,bool dontLookForFullStack = true) // First look for partialy full stacks
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].CheckIfItHasItem())
                if (slots[i].CheckMatchingName(_name))
                {
                    if (dontLookForFullStack) // If I'm cheking only partially full stacks
                        if (slots[i].GetItemInSlot().CheckIfStackIsFull()) // If the stack is full skip it
                            continue;

                    int aux = _amount;
                    _amount -= slots[i].GetItemInSlot().currentStack; // Substract the amount of resources spent form ccurent stack
                    slots[i].GetItemInSlot().TakeFromStack(aux); // Take the resources from the slot

                    if (_amount <= 0) // if enough resources have been taken return
                        return;
                }
            if (i == slots.Length - 1) // If I reached end of inventory
                if (_amount > 0) // If I haven't found enough resurces
                {
                    dontLookForFullStack = false; // Set to false so that I look for full stacks
                    i = -1; // Set to -1 so that I start looking from the begining of the inventory
                }

        }
    }


    public Item FindItemOfType(Item itemToFind)
    {
        foreach (InventorySlot slot in slots) // All slots in the inventory
            if (slot.CheckIfItHasItem())   // Check if there is an item in slot
                if (slot.CheckMatchingName(itemToFind.name)) // Check if the item in slot has the same type
                    return slot.GetItemInSlot(); // return the item if the types match

        return null;
    }

}
