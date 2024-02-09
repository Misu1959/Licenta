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
        handSlot.SetSlotType(EquipmentData.Type.hand); // Set hand slot type

        bodySlot = equipmentPanel.GetChild(1).GetComponent<EquipmentSlot>(); // Set body slot
        bodySlot.SetSlotType(EquipmentData.Type.body); // Set body slot type

        headSlot = equipmentPanel.GetChild(2).GetComponent<EquipmentSlot>(); // Set head slot
        headSlot.SetSlotType(EquipmentData.Type.head); // Set head slot type
    }

    public bool SetEquipment(Item_Base equipmentToAdd,bool unequipCurrent, bool keepSelected)
    {
        ItemUI equipmentUIToAdd = equipmentToAdd.GetComponent < ItemUI>();
        
        if (equipmentToAdd.GetComponent<Item>()) // If not an UI item
        {
            equipmentUIToAdd = ItemsManager.instance.CreateItemUI(equipmentToAdd.GetComponent<Item>());
            Destroy(equipmentToAdd.gameObject);
        }

        if (!unequipCurrent)
            return EquipItem(equipmentUIToAdd);
        else
        {
            UnequipItem(equipmentToAdd.GetEquipmentData().equipmentType, keepSelected);
            return EquipItem(equipmentUIToAdd);
        }
    }

    public bool EquipItem(ItemUI itemToEquip)
    {
        InventorySlot slotToSet = null;

        switch (itemToEquip.GetEquipmentData().equipmentType)
        {
            case EquipmentData.Type.hand:
                {
                    slotToSet = handSlot;
                    break;
                }
            case EquipmentData.Type.body:
                {
                    if (itemToEquip.GetComponent<Storage>())
                        InventoryManager.instance.DisplayBackpack(itemToEquip.GetComponent<Storage>());

                    slotToSet = bodySlot;
                    break;
                }
            case EquipmentData.Type.head:
                {
                    slotToSet = headSlot;
                    break;
                }
        }

        if (!slotToSet.CheckIfItHasItem())
        {
            InventoryManager.instance.AddItemToSlot(slotToSet, itemToEquip);
            return true;
        }
        else
            return false;
    }

    public void UnequipItem(EquipmentData.Type equipmentType, bool keepSelected)
    {
        InventorySlot slotToSet = null;

        switch (equipmentType)
        {
            case EquipmentData.Type.hand:
                {
                    if (!GetHandItem()) return;

                    // Cancel the action when unequiping the tool
                    if (PlayerActionManagement.instance.currentAction >= PlayerActionManagement.Action.chop &&
                        PlayerActionManagement.instance.currentAction <= PlayerActionManagement.Action.mine)
                        PlayerActionManagement.instance.CancelAction();

                    slotToSet = handSlot;
                    break;

                }
                case EquipmentData.Type.body:
                {
                    if (!GetBodyItem()) return;

                    if (bodySlot.GetItemInSlot().GetComponent<Storage>())
                        InventoryManager.instance.DisplayBackpack();

                    slotToSet = bodySlot;
                    break;
                }
                case EquipmentData.Type.head:
                {
                    if (!GetHeadItem()) return;

                    slotToSet = headSlot;
                    break;
                }
        }

        if (!keepSelected)
        {
            if (!slotToSet.GetItemInSlot().GetComponent<Storage>())
                InventoryManager.instance.AddItemToInventory(slotToSet.GetItemInSlot());
            else
                InventoryManager.instance.DropItem(slotToSet.GetItemInSlot(), PlayerStats.instance.transform.position);
        }
        else
            InventoryManager.instance.SetSelectedItem(slotToSet.GetItemInSlot());
    }


    public void ReplenishItem(ItemData.Name oldItemName)
    {
        ItemUI newItem = InventoryManager.instance.FindSpecificItem(oldItemName);
        
        if(newItem)
            SetEquipment(newItem, true, false);
    }

    public ItemUI GetHandItem() { return handSlot.CheckIfItHasItem() == true ? handSlot.GetItemInSlot() : null; }

    public ItemUI GetBodyItem() { return bodySlot.CheckIfItHasItem() == true ? bodySlot.GetItemInSlot() : null; }

    public ItemUI GetHeadItem() { return headSlot.CheckIfItHasItem() == true ? headSlot.GetItemInSlot() : null; }

    public Storage BackpackStorage()    
    {
        if(GetBodyItem())
            if(GetBodyItem().GetComponent<Storage>())
                return GetBodyItem().GetComponent<Storage>();
        
        return null;

    }
}
