using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class EquipmentSlot : InventorySlot
{
    private Equipment.Type slotType;

    void Update()
    {
        MouseOver();
    }

    public override void MouseOver()
    {
        if (!isMouseOver)
            return;

        if (!itemInSlot) // If I don't have an item in slot
        {
            if (!InventoryManager.instance.selectedItem) // If I don't have an item selected
                PopUpManager.instance.ShowMousePopUp();
            else // If I have an item selected
                if (!InventoryManager.instance.selectedItem.GetComponent<Equipment>()) // If the item selected is not an equipment
                PopUpManager.instance.ShowMousePopUp("RMB - cancel", 10);
            else // If the selected item is an equipment
            {
                if (!CheckForMatchingType(InventoryManager.instance.selectedItem.GetComponent<Equipment>())) // If the equipment type does not match
                    PopUpManager.instance.ShowMousePopUp("RMB - cancel", 10);
                else // If the equipment type match
                    PopUpManager.instance.ShowMousePopUp("LMB  - equip\nRMB - cancel", 10);
            }
        }
        else  // If I have an item in slot
        {
            if (!InventoryManager.instance.selectedItem) // If I don't have an item selected
                PopUpManager.instance.ShowMousePopUp("LMB  - select\nRMB - unequip", 10);
            else // If I have an item selected
            {
                if (!InventoryManager.instance.selectedItem.GetComponent<Equipment>()) // If the item selected is not an equipment
                    PopUpManager.instance.ShowMousePopUp("RMB - cancel", 10);
                else // If the selected item is an equipment
                {
                    if (!CheckForMatchingType(InventoryManager.instance.selectedItem.GetComponent<Equipment>())) // If the equipment type does not match
                        PopUpManager.instance.ShowMousePopUp("RMB - cancel", 10);
                    else // If the equipment type match
                        PopUpManager.instance.ShowMousePopUp("LMB  - swap\nRMB - cancel", 10);
                }
            }
        }

    }

    public override void SetItemInSlot(Item newItem)
    {
        if (!newItem) // If item is null return
        {
            itemInSlot = newItem; // set item in slot
            return;
        }

        if (!newItem.GetComponent<Equipment>()) // If the item is not an equipment return
            return;

        if (!CheckForMatchingType(newItem.GetComponent<Equipment>())) // If the equipment doesn't match slot return
            return;
        
        itemInSlot = newItem; // set item in slot
        itemInSlot.transform.SetParent(transform); // set the slot as item parent
        itemInSlot.transform.localPosition = Vector2.zero; // set it's position to Vector2.zero

        if (InventoryManager.instance.CheckSelecteditem(newItem)) // If the item is selected 
            InventoryManager.instance.SetSelectedItem(null); // Remove selected item

        PopUpManager.instance.ShowMousePopUp();
    }

    protected override void SelectItemFromSlot()
    {
        if (!InteractionManager.canInteract) // if I can't interact
            return;

        if (!InventoryManager.instance.selectedItem) // If I don't have an item selected already
        {
            if (Input.GetMouseButtonDown(0)) // If I press LMB
                EquipmentManager.instance.UnequipItem(slotType, true); //Unequip item and keep it selected
            else if (Input.GetMouseButtonDown(1)) // If I press RMB
                EquipmentManager.instance.UnequipItem(slotType); // unequip the item and set it in inventory
        }
        else // If I have an item selected already
        {
            if (Input.GetMouseButtonDown(0))// If I press LMB
            {
                if (!InventoryManager.instance.selectedItem.GetComponent<Equipment>()) // If the selected item is not an equipment
                    return;

                if (CheckForMatchingType(InventoryManager.instance.selectedItem.GetComponent<Equipment>())) // If the selected item match this slot
                {
                    Item aux = itemInSlot;

                    EquipmentManager.instance.SetEquipment(InventoryManager.instance.selectedItem.GetComponent<Equipment>(),true); // Equip selected item and unequip current item
                    InventoryManager.instance.SetSelectedItem(aux); // Select the item that was equiped
                }
            }
        }

    }

    public void SetSlotType(Equipment.Type _type)
    {
        slotType = _type; // Set slot type
    }

    private bool CheckForMatchingType(Equipment equipment)
    {
        // return if the Equipment type match slot type or not
        return (slotType != equipment.equipmentType) ? false : true;
    }
}
