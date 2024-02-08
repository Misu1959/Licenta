using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    protected ItemUI itemInSlot;
    protected bool isMouseOver;

    void Update() { MouseOver(); }

    public virtual void OnPointerDown(PointerEventData eventData)
    {
        if (!InteractionManager.CanPlayerInteractWithUI()) return;

        if (!itemInSlot) // If I don't have an item in slot
        {

            if (InventoryManager.instance.selectedItemSlot.CheckIfItHasItem()) // If I have an item selected
            {
                if (InventoryManager.instance.selectedItemSlot.GetItemInSlot().GetComponent<Storage>()) return;

                if (Input.GetMouseButtonDown(0)) // If I press LMB
                    InventoryManager.instance.AddItemToSlot(this, InventoryManager.instance.selectedItemSlot.GetItemInSlot());
            }
        }
        else// If I have an item in slot
        {
            if (!InventoryManager.instance.selectedItemSlot.CheckIfItHasItem()) // If I don't have an item selected
            {
                if (Input.GetMouseButtonDown(0)) // If I press LMB
                {
                    ItemUI itemToSelect = itemInSlot;
                    int amount = itemInSlot.GetItemData().currentStack;

                    if (Input.GetKey(KeyCode.LeftControl) && itemInSlot.GetItemData().currentStack > 1)
                        amount = 1;
                    else if (Input.GetKey(KeyCode.LeftShift) && itemInSlot.GetItemData().currentStack > 1)
                        amount = amount % 2 == 0 ? amount / 2 : amount / 2 + 1;

                    if (amount != itemInSlot.GetItemData().currentStack)
                    {
                        itemToSelect = ItemsManager.instance.CreateItemUI(itemInSlot.GetItemData(), amount);
                        itemInSlot.TakeFromStack(amount);
                    }
                    InventoryManager.instance.SetSelectedItem(itemToSelect);

                }
                else if (Input.GetMouseButtonDown(1)) // If I press RMB
                {
                    itemInSlot.OnRightMouseButtonPressed();

                    if (itemInSlot?.GetComponent<EquipmentUI>())
                        InventoryManager.instance.RemoveItemFromSlot(this);
                }
            }
            else // If I have an Item selected
            {
                if (Input.GetMouseButtonDown(0))// If I press LMB
                {
                    if (InventoryManager.instance.selectedItemSlot.GetItemInSlot().GetComponent<Storage>()) return;

                    if (CheckMatchingName(InventoryManager.instance.selectedItemSlot.GetItemInSlot().GetItemData().name))
                    {
                        if (itemInSlot.CheckIfStackIsFull() || InventoryManager.instance.selectedItemSlot.GetItemInSlot().CheckIfStackIsFull())
                            InventoryManager.instance.SwapTwoSlots(this);
                        else
                        {
                            int dif = itemInSlot.GetItemData().maxStack - itemInSlot.GetItemData().currentStack;

                            if (InventoryManager.instance.selectedItemSlot.GetItemInSlot().GetItemData().currentStack < dif)
                                dif = InventoryManager.instance.selectedItemSlot.GetItemInSlot().GetItemData().currentStack;

                            itemInSlot.AddToStack(dif);
                            InventoryManager.instance.selectedItemSlot.GetItemInSlot().TakeFromStack(dif);

                        }
                    }
                    else
                        InventoryManager.instance.SwapTwoSlots(this);
                }
            }

        }
    }

    public void OnPointerEnter(PointerEventData eventData) { isMouseOver = true; }
    public void OnPointerExit(PointerEventData eventData)
    {
        isMouseOver = false;
        PopUpManager.instance.ShowMousePopUp();
    }

    public virtual void MouseOver()
    {
        if (!isMouseOver) return;
        if (!InteractionManager.CanPlayerInteractWithUI()) return;

        if (!itemInSlot) // If I don't have an item in slot
        {
            if (InventoryManager.instance.selectedItemSlot.CheckIfItHasItem()) // If I have an item selected
            {
                if (InventoryManager.instance.selectedItemSlot.GetItemInSlot().GetComponent<Storage>())
                    PopUpManager.instance.ShowMousePopUp("RMB - cancel", PopUpManager.PopUpPriorityLevel.medium);
                else
                    PopUpManager.instance.ShowMousePopUp("LMB  - place\nRMB - cancel", PopUpManager.PopUpPriorityLevel.medium);
            }
            else
                PopUpManager.instance.ShowMousePopUp();
        }
        else // If I have an item in slot
        {
            if (!InventoryManager.instance.selectedItemSlot.CheckIfItHasItem()) // If I don't have an item selected
            {
                if (itemInSlot.GetComponent<FoodUI>())
                    PopUpManager.instance.ShowMousePopUp("LMB  - select\nRMB - eat", PopUpManager.PopUpPriorityLevel.medium);
                else if (itemInSlot.GetComponent<EquipmentUI>())
                    PopUpManager.instance.ShowMousePopUp("LMB  - select\nRMB - equip", PopUpManager.PopUpPriorityLevel.medium);
                else
                    PopUpManager.instance.ShowMousePopUp("LMB  - select", PopUpManager.PopUpPriorityLevel.medium);
            }
            else // If I have an item selected
            {
                if (!CheckMatchingName(InventoryManager.instance.selectedItemSlot.GetItemInSlot().GetItemData().name)) // if the items are different
                {
                    if (InventoryManager.instance.selectedItemSlot.GetItemInSlot().GetComponent<Storage>())
                        PopUpManager.instance.ShowMousePopUp("RMB - cancel", PopUpManager.PopUpPriorityLevel.medium);
                    else
                        PopUpManager.instance.ShowMousePopUp("LMB  - swap\nRMB - cancel", PopUpManager.PopUpPriorityLevel.medium);
                }
                else // If the items have the same name
                {
                    if (InventoryManager.instance.selectedItemSlot.GetItemInSlot().CheckIfStackIsFull() || itemInSlot.CheckIfStackIsFull()) // If any of the stack are full
                        PopUpManager.instance.ShowMousePopUp("LMB  - swap\nRMB - cancel", PopUpManager.PopUpPriorityLevel.medium);
                    else // If neither stacks are full
                        PopUpManager.instance.ShowMousePopUp("LMB  - add\nRMB - cancel", PopUpManager.PopUpPriorityLevel.medium);
                }
            }
        }

    }

    public ItemUI GetItemInSlot() { return itemInSlot; }

    public void SetItemInSlot(ItemUI newItemInSlot) { itemInSlot = newItemInSlot; }

    public bool CheckIfItHasItem() { return itemInSlot == null ? false : true; }

    public bool CheckMatchingName(ItemData.Name nameToCompare) { return itemInSlot?.GetItemData().name != nameToCompare ? false : true; }

    public bool IsBackpackSlot() {   return (transform.parent != InventoryManager.instance.backpack) ? false : true; }

    public bool IsChestSlot() {   return (transform.parent != InventoryManager.instance.chest) ? false : true; }

}
