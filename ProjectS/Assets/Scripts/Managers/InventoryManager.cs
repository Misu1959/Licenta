using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance;

    public Transform inventory;
    private Transform[] slots = new Transform[15];
    
    [HideInInspector] public Item selectedItem;

    void Start()
    {
        instance = this;
    
        SetSlots();
    }

    void SetSlots()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            int j = i;
            slots[i] = inventory.GetChild(i);
            slots[i].GetComponent<Button>().onClick.AddListener(()=> AddItemToEmptySlot(slots[j]));
        }
    }

    Transform FindFreeSlot()
    {
        foreach (Transform slot in slots)
        {
            if (slot.childCount == 0) // If the slot is empty
                return slot; // Return an empty slot
        }

        return null;
    }
    public Transform FindSlot(GameObject itemToAdd)
    {
        Transform emptySlot = null;
        foreach (Transform slot in slots)
        {
            if (!emptySlot) // if we haven't found an empty slot yet
                if (slot.childCount == 0) // If the slot is empty
                    emptySlot = slot; // Set it to the aux var

            if(slot.childCount>0)   // Check if there is an item in slot
                if(slot.GetChild(0).GetComponent<Item>().type == itemToAdd.GetComponent<Item>().type) // Check if the items have the same type
                    if(!slot.GetChild(0).GetComponent<Item>().CheckIfStackIsFull())  // If the item in slot does't have a full stack return it
                        return slot;

        }

        return emptySlot; // If we haven't found a slot that is partialy full return an empty slot if this exist
    }

    public void AddItemToSlot(GameObject itemToAdd)
    {
        Transform slot = FindSlot(itemToAdd);
        if (slot?.childCount == 0) // if the slot is empty
        {
            // Create the ItemUi and add it to the free slot
            itemToAdd.GetComponent<Item>().CreateItemUI(slot, itemToAdd.GetComponent<Item>().currentStack);
            CraftingManager.instance.SetTooltipCraftButton();

            Destroy(itemToAdd); //Destroy the item
        }
        else if(slot?.childCount > 0)// If the slot is partially full
        {
            Item itemInSlot = slot.GetChild(0).GetComponent<Item>(); // I get the item that is in slot
            int dif = itemInSlot.maxStack- itemInSlot.currentStack; // I calculate how much can I add to the item stack

            if (itemToAdd.GetComponent<Item>().currentStack < dif) // I check If I can't add both's item's stack together in one
            {
                itemInSlot.AddToStack(itemToAdd.GetComponent<Item>().currentStack);
                Destroy(itemToAdd);
            }
            else // If not I add them until the item that is in stock has a full stack
            {
                itemToAdd.GetComponent<Item>().TakeFromStack(dif);
                itemInSlot.AddToStack(dif);

                AddItemToSlot(itemToAdd); // I call the function again so that I look for a new slot
            }
            CraftingManager.instance.SetTooltipCraftButton();
        }
        else if(slot == null) // If I haven't found an available slot
        {
            // I drop the item on the floor
            itemToAdd.GetComponent<Item>().Drop(PlayerStats.instance.gameObject.transform.position);
        }

    }
    void AddItemToEmptySlot(Transform slot)
    {
        if ((!selectedItem) || slot.childCount > 0)
            return;

        selectedItem.transform.SetParent(slot);
        selectedItem.transform.localPosition = Vector2.zero;
        selectedItem.GetComponent<Image>().raycastTarget = true;
        selectedItem = null;
    }

    public void SwapTwoSlots(Item itemToSwap)
    {
        selectedItem.transform.SetParent(itemToSwap.transform.parent);
        selectedItem.transform.localPosition = Vector2.zero;
        selectedItem.GetComponent<Image>().raycastTarget = true;


        selectedItem = itemToSwap;

        selectedItem.transform.SetParent(inventory);
        selectedItem.GetComponent<Image>().raycastTarget = false;
    }

    public void SetBackToSlot()
    {
        if (!selectedItem) // If I have't an item selected return
            return;

        // Set the item selected to a free slot

        selectedItem.transform.SetParent(FindFreeSlot());
        selectedItem.transform.localPosition = Vector2.zero;
        selectedItem.GetComponent<Image>().raycastTarget = true;
        selectedItem = null;

        PopUpManager.instance.ShowMousePopUp();
    }

    public int AmountOwnedOfType(string _type)
    {
        int totalAmount = 0;

        foreach (Transform slot in slots)
            if (slot.childCount > 0)
                if (slot.GetChild(0).GetComponent<Item>().type == _type)
                    totalAmount += slot.GetChild(0).GetComponent<Item>().currentStack;

        return totalAmount;
    }

    public void SpendResources(string _type, int _amount,bool dontLookForFullStack = true) // First look for partialy full stacks
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].childCount > 0)
                if (slots[i].GetChild(0).GetComponent<Item>().type == _type)
                {
                    if (dontLookForFullStack) // If I'm cheking only partially full stacks
                        if (slots[i].GetChild(0).GetComponent<Item>().CheckIfStackIsFull()) // If the stack is full skip it
                            continue;

                    int aux = _amount;
                    _amount -= slots[i].GetChild(0).GetComponent<Item>().currentStack; // Substract the amount of resources spent form ccurent stack
                    slots[i].GetChild(0).GetComponent<Item>().TakeFromStack(aux); // Take the resources from the slot

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
        foreach (Transform slot in slots) // All slots in the inventory
            if (slot.childCount > 0)   // Check if there is an item in slot
                if (slot.GetChild(0).GetComponent<Item>().type == itemToFind.type) // Check if the item in slot has the same type
                    return slot.GetChild(0).GetComponent<Item>(); // return the item if the types match

        return null;
    }

}
