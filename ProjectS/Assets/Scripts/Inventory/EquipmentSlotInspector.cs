using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EquipmentSlot))]
public class EquipmentSlotInspector : InventorySlotInspector
{
    public override void MouseOver()
    {
        if (!isMouseOver) return;
        if (!InteractionManager.CanPlayerInteractWithUI()) return;

        if (!inventorySlot.CheckIfItHasItem()) // If I don't have an item in slot
        {
            if (InventoryManager.instance.selectedItemSlot.CheckIfItHasItem()) // If I don't have an item selected
            {
                if (!InventoryManager.instance.selectedItemSlot.GetItemInSlot().GetComponent<EquipmentUI>()) // If the item selected is not an equipment
                    PopUpManager.instance.ShowMousePopUp("RMB - cancel", PopUpManager.PopUpPriorityLevel.medium);
                else // If the selected item is an equipment
                {
                    if (!inventorySlot.GetComponent<EquipmentSlot>().CheckForMatchingType(InventoryManager.instance.selectedItemSlot.GetItemInSlot().GetEquipmentData().equipmentType)) // If the equipment type does not match
                        PopUpManager.instance.ShowMousePopUp("RMB - cancel", PopUpManager.PopUpPriorityLevel.medium);
                    else // If the equipment type match
                        PopUpManager.instance.ShowMousePopUp("LMB  - equip\nRMB - cancel", PopUpManager.PopUpPriorityLevel.medium);
                }
            }

        }
        else  // If I have an item in slot
        {
            if (!InventoryManager.instance.selectedItemSlot.GetItemInSlot()) // If I don't have an item selected
                PopUpManager.instance.ShowMousePopUp("LMB  - select\nRMB - unequip", PopUpManager.PopUpPriorityLevel.medium);
            else // If I have an item selected
            {
                if (!InventoryManager.instance.selectedItemSlot.GetItemInSlot().GetComponent<EquipmentUI>()) // If the item selected is not an equipment
                    PopUpManager.instance.ShowMousePopUp("RMB - cancel", PopUpManager.PopUpPriorityLevel.medium);
                else // If the selected item is an equipment
                {
                    if (!inventorySlot.GetComponent<EquipmentSlot>().CheckForMatchingType(InventoryManager.instance.selectedItemSlot.GetItemInSlot().GetEquipmentData().equipmentType)) // If the equipment type does not match
                        PopUpManager.instance.ShowMousePopUp("RMB - cancel", PopUpManager.PopUpPriorityLevel.medium);
                    else // If the equipment type match
                        PopUpManager.instance.ShowMousePopUp("LMB  - swap\nRMB - cancel", PopUpManager.PopUpPriorityLevel.medium);
                }
            }
        }

    }
}
