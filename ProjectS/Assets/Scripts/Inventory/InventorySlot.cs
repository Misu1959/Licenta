using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour,IPointerDownHandler,IPointerEnterHandler,IPointerExitHandler
{
    protected Item itemInSlot;
    protected bool isMouseOver;

    void Update()
    {
        MouseOver();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!itemInSlot)
        {
            if (Input.GetMouseButtonDown(0))
                SetItemInSlot(InventoryManager.instance.selectedItem);
        }
        else
            SelectItemFromSlot();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isMouseOver = true;

    }
    public void OnPointerExit(PointerEventData eventData)
    {
        isMouseOver = false;
        PopUpManager.instance.ShowMousePopUp();
    }

    public virtual void MouseOver()
    {
        if (!isMouseOver)
            return;

        if (!itemInSlot) // If I don't have an item in slot
        {
            if (InventoryManager.instance.selectedItem) // If I have an item selected
                PopUpManager.instance.ShowMousePopUp("LMB  - place\nRMB - cancel", 10);
            else
                PopUpManager.instance.ShowMousePopUp();
        }
        else // If I have an item in slot
        {
            if (!InventoryManager.instance.selectedItem) // If I don't have an item selected
            {
                if (itemInSlot.GetComponent<Food>())
                    PopUpManager.instance.ShowMousePopUp("LMB  - select\nRMB - eat", 10);
                else if (itemInSlot.GetComponent<Equipment>())
                    PopUpManager.instance.ShowMousePopUp("LMB  - select\nRMB - equip", 10);
                else
                    PopUpManager.instance.ShowMousePopUp("LMB  - select", 10);
            }
            else // If I have an item selected
            {
                if (!CheckMatchingName(InventoryManager.instance.selectedItem.type)) // if the items are different
                { 
                    PopUpManager.instance.ShowMousePopUp("LMB  - swap\nRMB - cancel", 10);
                }
                else // If the items have the same name
                {
                    if (InventoryManager.instance.selectedItem.CheckIfStackIsFull() || itemInSlot.CheckIfStackIsFull()) // If any of the stack are full
                        PopUpManager.instance.ShowMousePopUp("LMB  - swap\nRMB - cancel", 10);
                    else // If neither stacks are full
                        PopUpManager.instance.ShowMousePopUp("LMB  - add\nRMB - cancel", 10);
                }
            }
        }

    }

    public virtual void SetItemInSlot(Item newItem)
    {
        itemInSlot = newItem; // set the item in slot
        if (itemInSlot) // If the item is not null
        {
            itemInSlot.transform.SetParent(transform); // set the slot as item parent
            itemInSlot.transform.localPosition = Vector2.zero; // set it's position to Vector2.zero

            if (InventoryManager.instance.CheckSelecteditem(newItem)) // If the item is selected 
                InventoryManager.instance.SetSelectedItem(null); // Remove selected item

            PopUpManager.instance.ShowMousePopUp();
        }
    }

    protected virtual void SelectItemFromSlot()
    {
        if (!InteractionManager.canInteract) // if I can't interact
            return;

        if (!InventoryManager.instance.selectedItem) // If I don't have an item selected already
        {
            if (Input.GetMouseButtonDown(0)) // If I press LMB
            {
                if (Input.GetKey(KeyCode.LeftControl) && itemInSlot.currentStack > 1)
                {
                    itemInSlot.CreateItemUI(1);
                    itemInSlot.TakeFromStack(1);
                }
                else if (Input.GetKey(KeyCode.LeftShift) && itemInSlot.currentStack > 1)
                {
                    int amount = itemInSlot.currentStack % 2 == 0 ? itemInSlot.currentStack / 2 : itemInSlot.currentStack / 2 + 1;

                    itemInSlot.CreateItemUI(amount);
                    itemInSlot.TakeFromStack(amount);
                }
                else
                {
                    InventoryManager.instance.SetSelectedItem(itemInSlot);
                    SetItemInSlot(null);
                }
            }
            else if (Input.GetMouseButtonDown(1)) // If I press RMB
            {
                if (itemInSlot.GetComponent<Food>()) // If item in slot is food eat it
                    itemInSlot.GetComponent<Food>().Consume();
                else if (itemInSlot.GetComponent<Equipment>()) // If item in slot is equipment equip it
                {
                    EquipmentManager.instance.SetEquipment(itemInSlot.GetComponent<Equipment>());
                    SetItemInSlot(null);
                }
            }
        }
        else // If I have an item selected already
        {
            if (Input.GetMouseButtonDown(0))// If I press LMB
            {
                if (CheckMatchingName(InventoryManager.instance.selectedItem.type))
                {
                    if (itemInSlot.CheckIfStackIsFull() || InventoryManager.instance.selectedItem.CheckIfStackIsFull())
                        InventoryManager.instance.SwapTwoSlots(this);
                    else
                    {
                        int dif = InventoryManager.instance.selectedItem.currentStack + itemInSlot.currentStack - itemInSlot.maxStack;

                        if (dif > 0)
                        {
                            InventoryManager.instance.selectedItem.TakeFromStack(dif);
                            itemInSlot.AddToStack(dif);
                        }
                        else
                        {
                            itemInSlot.AddToStack(InventoryManager.instance.selectedItem.currentStack);
                            Destroy(InventoryManager.instance.selectedItem.gameObject);
                        }
                    }
                }
                else
                    InventoryManager.instance.SwapTwoSlots(this);
            }
        }
        
    }

    public Item GetItemInSlot() { return itemInSlot; } // return the item in slot

    public bool CheckIfItHasItem()
    {
        // return wether the slot has item or not
        return itemInSlot == null ? false : true;
    }

    public bool CheckMatchingName(string nameToCompare)
    {
        // return wether the name of the item matches the name of the item in slot
        return itemInSlot.type != nameToCompare ? false : true;

    }
}
