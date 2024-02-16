using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(InventorySlot))]
public class InventorySlotInspector : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler
{

    protected InventorySlot inventorySlot;
    protected bool isMouseOver;

    private void Awake() => inventorySlot = GetComponent<InventorySlot>();
    private void Update() => MouseOver();

    public void OnPointerEnter(PointerEventData eventData) => isMouseOver = true;
    public void OnPointerExit(PointerEventData eventData) => isMouseOver = false;

    public virtual void MouseOver()
    {
        if (IsSelectedItemSlot()) return;

        if (!isMouseOver) return;
        if (!InteractionManager.CanPlayerInteractWithUI()) return;

        if (!inventorySlot.CheckIfItHasItem()) // If I don't have an item in slot
        {
            if (InventoryManager.instance.selectedItemSlot.CheckIfItHasItem()) // If I have an item selected
            {
                if (InventoryManager.instance.selectedItemSlot.GetItemInSlot().GetComponent<Storage>())
                    PopUpManager.instance.ShowMousePopUp("RMB - cancel", PopUpManager.PopUpPriorityLevel.medium);
                else
                    PopUpManager.instance.ShowMousePopUp("LMB  - place\nRMB - cancel", PopUpManager.PopUpPriorityLevel.medium);
            }
        }
        else // If I have an item in slot
        {
            if (!InventoryManager.instance.selectedItemSlot.CheckIfItHasItem()) // If I don't have an item selected
            {
                if (inventorySlot.GetItemInSlot().GetComponent<FoodUI>())
                    PopUpManager.instance.ShowMousePopUp("LMB  - select\nRMB - eat", PopUpManager.PopUpPriorityLevel.medium);
                else if (inventorySlot.GetItemInSlot().GetComponent<EquipmentUI>())
                    PopUpManager.instance.ShowMousePopUp("LMB  - select\nRMB - equip", PopUpManager.PopUpPriorityLevel.medium);
                else
                    PopUpManager.instance.ShowMousePopUp("LMB  - select", PopUpManager.PopUpPriorityLevel.medium);
            }
            else // If I have an item selected
            {
                if (!inventorySlot.CheckMatchingName(InventoryManager.instance.selectedItemSlot.GetItemInSlot().GetItemData().name)) // if the items are different
                {
                    if (InventoryManager.instance.selectedItemSlot.GetItemInSlot().GetComponent<Storage>())
                        PopUpManager.instance.ShowMousePopUp("RMB - cancel", PopUpManager.PopUpPriorityLevel.medium);
                    else
                        PopUpManager.instance.ShowMousePopUp("LMB  - swap\nRMB - cancel", PopUpManager.PopUpPriorityLevel.medium);
                }
                else // If the items have the same name
                {
                    if (InventoryManager.instance.selectedItemSlot.GetItemInSlot().CheckIfStackIsFull() || inventorySlot.GetItemInSlot().CheckIfStackIsFull()) // If any of the stack are full
                        PopUpManager.instance.ShowMousePopUp("LMB  - swap\nRMB - cancel", PopUpManager.PopUpPriorityLevel.medium);
                    else // If neither stacks are full
                        PopUpManager.instance.ShowMousePopUp("LMB  - add\nRMB - cancel", PopUpManager.PopUpPriorityLevel.medium);
                }
            }
        }

    }

    private bool IsSelectedItemSlot()
    {
        if (inventorySlot != InventoryManager.instance.selectedItemSlot) return false;
        if (!inventorySlot.CheckIfItHasItem()) return false;

        if (MyMethods.CheckIfMouseIsOverUI() || MyMethods.CheckIfMouseIsOverItem())
            PopUpManager.instance.ShowMousePopUp("RMB - Cancel", PopUpManager.PopUpPriorityLevel.medium);
        else
            PopUpManager.instance.ShowMousePopUp("LMB - Drop\nRMB - Cancel", PopUpManager.PopUpPriorityLevel.medium);

        return true;
    }

}
