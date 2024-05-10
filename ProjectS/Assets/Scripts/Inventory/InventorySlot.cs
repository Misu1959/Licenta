using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour, IPointerDownHandler
{
    protected ItemUI itemInSlot;

    public virtual void OnPointerDown(PointerEventData eventData)
    {
        if (!InteractionManager.instance.CanPlayerInteractWithUI()) return;

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

                    if (CheckMatchingName(InventoryManager.instance.selectedItemSlot.GetItemInSlot().GetItemData().objectName))
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

    public ItemUI GetItemInSlot() => itemInSlot; 
    public void SetItemInSlot(ItemUI newItemInSlot) => itemInSlot = newItemInSlot;


    public bool CheckIfItHasItem() => itemInSlot == null ? false : true; 
    public bool CheckMatchingName(ObjectName nameToCompare) => itemInSlot?.GetItemData().objectName != nameToCompare ? false : true;


    public bool IsBackpackSlot() => (transform.parent != InventoryManager.instance.backpackPanel) ? false : true;
    public bool IsChestSlot() => (transform.parent != InventoryManager.instance.chestPanel) ? false : true;

}
