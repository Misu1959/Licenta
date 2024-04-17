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
        handSlot.SetSlotType(EquipmentType.hand); // Set hand slot type

        bodySlot = equipmentPanel.GetChild(1).GetComponent<EquipmentSlot>(); // Set body slot
        bodySlot.SetSlotType(EquipmentType.body); // Set body slot type

        headSlot = equipmentPanel.GetChild(2).GetComponent<EquipmentSlot>(); // Set head slot
        headSlot.SetSlotType(EquipmentType.head); // Set head slot type
    }

    public bool SetEquipment(Item_Base equipmentToAdd,bool unequipCurrent, bool keepSelected)
    {
        ItemUI equipmentUIToAdd = equipmentToAdd.GetComponent <ItemUI>();
        
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

    private bool EquipItem(ItemUI itemToEquip)
    {
        InventorySlot slotToSet = null;

        switch (itemToEquip.GetEquipmentData().equipmentType)
        {
            case EquipmentType.hand:
                {
                    slotToSet = handSlot;
                    break;
                }
            case EquipmentType.body:
                {
                    if (itemToEquip.GetComponent<Storage>())
                        InventoryManager.instance.DisplayBackpack(itemToEquip.GetComponent<Storage>());

                    Debug.Log("Equip body piece");

                    slotToSet = bodySlot;
                    break;
                }
            case EquipmentType.head:
                {
                    slotToSet = headSlot;
                    break;
                }
        }

        if (!slotToSet.CheckIfItHasItem())
        {
            Debug.Log("Final");
            InventoryManager.instance.AddItemToSlot(slotToSet, itemToEquip);
            return true;
        }
        else
            return false;
    }

    public void UnequipItem(EquipmentType equipmentType, bool keepSelected)
    {
        InventorySlot slotToSet = null;

        switch (equipmentType)
        {
            case EquipmentType.hand:
                {
                    if (!GetHandItem()) return;

                    // Cancel the action when unequiping the tool
                    if ((PlayerBehaviour.instance.currentAction >= PlayerBehaviour.Action.chop &&
                        PlayerBehaviour.instance.currentAction <= PlayerBehaviour.Action.mine) ||
                        PlayerBehaviour.instance.currentAction == PlayerBehaviour.Action.attack
                        )
                        PlayerBehaviour.instance.CancelAction();

                    slotToSet = handSlot;
                    break;

                }
                case EquipmentType.body:
                {
                    if (!GetBodyItem()) return;

                    if (bodySlot.GetItemInSlot().GetComponent<Storage>())
                        InventoryManager.instance.DisplayBackpack();

                    Debug.Log("Unequip body piece");
                    slotToSet = bodySlot;
                    break;
                }
                case EquipmentType.head:
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


    public void ReplenishItem(ObjectName oldItemName)
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
