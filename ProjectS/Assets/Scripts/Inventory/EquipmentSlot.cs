using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class EquipmentSlot : InventorySlot
{
    private EquipmentData.Type slotType;

    private void Update()   {   MouseOver();    }

    public override void MouseOver()
    {
        if (!isMouseOver) return;
        if (!InteractionManager.CanPlayerInteractWithUI()) return;

        if (!itemInSlot) // If I don't have an item in slot
        {
            if (!InventoryManager.instance.selectedItemSlot.CheckIfItHasItem()) // If I don't have an item selected
                PopUpManager.instance.ShowMousePopUp();
            else // If I have an item selected
            {
                if (!InventoryManager.instance.selectedItemSlot.GetItemInSlot().GetComponent<EquipmentUI>()) // If the item selected is not an equipment
                    PopUpManager.instance.ShowMousePopUp("RMB - cancel", PopUpManager.PopUpPriorityLevel.medium);
                else // If the selected item is an equipment
                {
                    if (!CheckForMatchingType(InventoryManager.instance.selectedItemSlot.GetItemInSlot().GetEquipmentData().equipmentType)) // If the equipment type does not match
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
                    if (!CheckForMatchingType(InventoryManager.instance.selectedItemSlot.GetItemInSlot().GetEquipmentData().equipmentType)) // If the equipment type does not match
                        PopUpManager.instance.ShowMousePopUp("RMB - cancel", PopUpManager.PopUpPriorityLevel.medium);
                    else // If the equipment type match
                        PopUpManager.instance.ShowMousePopUp("LMB  - swap\nRMB - cancel", PopUpManager.PopUpPriorityLevel.medium);
                }
            }
        }

    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        if (!InteractionManager.CanPlayerInteractWithUI()) return;

        if (!itemInSlot) // If I don't have an item in slot
        {
            if (InventoryManager.instance.selectedItemSlot.CheckIfItHasItem()) // If I have an item selected
            {
                if (Input.GetMouseButtonDown(0))// If I press LMB
                {
                    if (!InventoryManager.instance.selectedItemSlot.GetItemInSlot().GetComponent<EquipmentUI>()) return; // If the selected item is not an equipment
                        

                    if (CheckForMatchingType(InventoryManager.instance.selectedItemSlot.GetItemInSlot().GetEquipmentData().equipmentType)) // If the selected item match this slot
                        EquipmentManager.instance.SetEquipment(InventoryManager.instance.selectedItemSlot.GetItemInSlot(),false, false); // Equip selected item
                }
            }
        }
        else // If I have an item in slot 
        {
            if (!InventoryManager.instance.selectedItemSlot.CheckIfItHasItem()) // If don't have an item selected
            {
                if (Input.GetMouseButtonDown(0)) // If I press LMB
                    EquipmentManager.instance.UnequipItem(slotType, true); //Unequip item and keep it selected
                else if (Input.GetMouseButtonDown(1)) // If I press RMB
                    EquipmentManager.instance.UnequipItem(slotType, false); // unequip the item and set it in inventory
            }
            else // If don't have an item selected
            {
                if (Input.GetMouseButtonDown(0))// If I press LMB
                {
                    if (!InventoryManager.instance.selectedItemSlot.GetItemInSlot().GetComponent<EquipmentUI>()) return; // If the selected item is not an equipment

                    if (CheckForMatchingType(InventoryManager.instance.selectedItemSlot.GetItemInSlot().GetEquipmentData().equipmentType)) // If the selected item match this slot
                        EquipmentManager.instance.SetEquipment(InventoryManager.instance.selectedItemSlot.GetItemInSlot(), true,true); // Equip selected item and unequip current item
                }
            }
        }
    }

    public void SetSlotType(EquipmentData.Type _type) { slotType = _type; }// Set slot type
                                                                           
    private bool CheckForMatchingType(EquipmentData.Type equipmentType) { return (slotType != equipmentType) ? false : true; }// check if the Equipment type match slot type or not
}
