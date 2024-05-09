using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EquipmentSlot : InventorySlot
{
    private EquipmentType slotType;

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
            if (!InventoryManager.instance.selectedItemSlot.CheckIfItHasItem()) // If I don't have an item selected
            {
                if (Input.GetMouseButtonDown(0)) // If I press LMB
                    EquipmentManager.instance.UnequipItem(slotType, true); //Unequip item and keep it selected
                else if (Input.GetMouseButtonDown(1)) // If I press RMB
                    EquipmentManager.instance.UnequipItem(slotType, false); // unequip the item and add it to inventory
            }
            else // If have an item selected
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

    public void SetSlotType(EquipmentType _type) => slotType = _type; // Set slot type
                                                                           
    public bool CheckForMatchingType(EquipmentType equipmentType) => (slotType != equipmentType) ? false : true; // check if the Equipment type match slot type or not
}
