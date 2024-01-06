using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System.Diagnostics.CodeAnalysis;

public class EquipmentManager : MonoBehaviour
{
    public static EquipmentManager instance;

    [SerializeField] private Transform equipmentPanel;

    private EquipmentSlot handSlot;
    private EquipmentSlot bodySlot;
    private EquipmentSlot headSlot;

    private void Start()
    {
        instance = this;
        SetEquipmentSlots();
    }

    private void SetEquipmentSlots()
    {
        handSlot = equipmentPanel.GetChild(0).GetComponent<EquipmentSlot>(); // Set hand slot
        handSlot.SetSlotType(Equipment.Type.hand); // Set hand slot type

        bodySlot = equipmentPanel.GetChild(1).GetComponent<EquipmentSlot>(); // Set body slot
        bodySlot.SetSlotType(Equipment.Type.body); // Set body slot type

        headSlot = equipmentPanel.GetChild(2).GetComponent<EquipmentSlot>(); // Set head slot
        headSlot.SetSlotType(Equipment.Type.head); // Set head slot type
    }

    public void SetEquipment(Equipment newEquipment, bool keepSelected = false)
    {

        UnequipItem(newEquipment.equipmentType,keepSelected); //Unequip the current item of type if there is one equiped
        switch (newEquipment.equipmentType)
        {
            case Equipment.Type.hand:
                {
                    handSlot.SetItemInSlot(newEquipment); // Equip the new hand item
                    break;
                }
            case Equipment.Type.body:
                {
                    bodySlot.SetItemInSlot(newEquipment); // Equip the new body item
                    break;
                }
            case Equipment.Type.head:
                {
                    headSlot.SetItemInSlot(newEquipment); // Equip the new head item
                    break;
                }

        }
    }
    public void UnequipItem(Equipment.Type equipmentType, bool keepSelected = false)
    {

        switch (equipmentType)
        {
            case Equipment.Type.hand:
                {
                    if (!handSlot.CheckIfItHasItem()) // If there is no item in hand there is nothing to unequip
                        return;

                    if(!keepSelected)
                        InventoryManager.instance.AddItemToSlot(handSlot.GetItemInSlot()); // Add the equiped hand item to inventory
                    else
                        InventoryManager.instance.SetSelectedItem(handSlot.GetItemInSlot()); // Select the current hand item

                    // Cancel the action when unequiping the tool
                    if (PlayerActionManagement.instance.currentAction >= PlayerActionManagement.Action.chop &&
                        PlayerActionManagement.instance.currentAction <= PlayerActionManagement.Action.mine)
                        PlayerActionManagement.instance.CancelAction();

                    handSlot.SetItemInSlot(null);
                    break;

                }
                case Equipment.Type.body:
                {
                    if (!bodySlot.CheckIfItHasItem()) // If there is no item in hand there is nothing to unequip
                        return;

                    if (!keepSelected)
                        InventoryManager.instance.AddItemToSlot(bodySlot.GetItemInSlot()); // Add the equiped body item to inventory
                    else
                        InventoryManager.instance.SetSelectedItem(bodySlot.GetItemInSlot()); // Select the current hand item

                    bodySlot.SetItemInSlot(null);

                    break;
                }
                case Equipment.Type.head:
                {
                    if (!headSlot.CheckIfItHasItem()) // If there is no item in hand there is nothing to unequip
                        return;
                    
                    if (!keepSelected)
                        InventoryManager.instance.AddItemToSlot(headSlot.GetItemInSlot());// Add the equiped head item to inventory
                    else
                        InventoryManager.instance.SetSelectedItem(bodySlot.GetItemInSlot()); // Select the current hand item

                    headSlot.SetItemInSlot(null);
                    break;
                }
        }
    }
    public Equipment GetHandItem()
    {
        if (handSlot.CheckIfItHasItem()) // Check if there is an item in hand slot
            return handSlot.GetItemInSlot().GetComponent<Equipment>(); // return the item in hand slot
        return null; // Return null if there is no item in hand slot
    }

    public Equipment GetBodyItem()
    {
        if (bodySlot.CheckIfItHasItem()) // Check if there is an item in body slot
            return bodySlot.GetItemInSlot().GetComponent<Equipment>(); // return the item in body slot
        return null; // Return null if there is no item in body slot
    }

    public Equipment GetHeadItem()
    {
        if (headSlot.CheckIfItHasItem()) // Check if there is an item in head slot
            return headSlot.GetItemInSlot().GetComponent<Equipment>(); // return the item in head slot
        return null; // Return null if there is no item in head slot
    }

}
