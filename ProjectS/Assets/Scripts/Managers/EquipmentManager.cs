using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class EquipmentManager : MonoBehaviour
{
    public static EquipmentManager instance;

    [SerializeField] private Transform equipmentPanel;

    private EquipmentSlot handSlot;
    private EquipmentSlot bodySlot;
    private EquipmentSlot headSlot;

    private void Awake()
    {
        instance = this;
        this.gameObject.SetActive(false);
    }

    private void Start() => SetEquipmentSlots();

    private void SetEquipmentSlots()
    {
        handSlot = equipmentPanel.GetChild(0).GetComponent<EquipmentSlot>(); // Set hand slot
        handSlot.SetSlotType(EquipmentType.hand); // Set hand slot type

        bodySlot = equipmentPanel.GetChild(1).GetComponent<EquipmentSlot>(); // Set body slot
        bodySlot.SetSlotType(EquipmentType.body); // Set body slot type

        headSlot = equipmentPanel.GetChild(2).GetComponent<EquipmentSlot>(); // Set head slot
        headSlot.SetSlotType(EquipmentType.head); // Set head slot type
    }

    public bool SetEquipment(Item_Base equipmentToAdd, bool unequipCurrent, bool keepSelected)
    {
        ItemUI equipmentUIToAdd = equipmentToAdd.GetComponent<ItemUI>();

        if (equipmentToAdd.GetComponent<Item>()) // If not an UI item
        {
            equipmentUIToAdd = ItemsManager.instance.CreateItemUI(equipmentToAdd.GetComponent<Item>());
            Destroy(equipmentToAdd.gameObject);
        }

        if (!unequipCurrent)
            return EquipItem(equipmentUIToAdd);
        else
        {
            StartCoroutine(SwapEquipment(equipmentUIToAdd,keepSelected));
            return true;
        }
    }

    private IEnumerator SwapEquipment(ItemUI itemToEquip, bool keepSelected)
    {
        UnequipItem(itemToEquip.GetEquipmentData().equipmentType, keepSelected);
        yield return new WaitForSeconds (.01f);
        EquipItem(itemToEquip);
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
            InventoryManager.instance.AddItemToSlot(slotToSet, itemToEquip);

            if (itemToEquip.GetComponent<TorchUI>())
                itemToEquip.GetComponent<TorchUI>().SetisEquiped(true);

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

                    if (GetHandItem().GetComponent<TorchUI>())
                        GetHandItem().GetComponent<TorchUI>().SetisEquiped(false);

                    slotToSet = handSlot;
                    break;

                }
            case EquipmentType.body:
                {
                    if (!GetBodyItem()) return;

                    if (bodySlot.GetItemInSlot().GetComponent<Storage>())
                        InventoryManager.instance.DisplayBackpack();

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

        if (newItem)
            SetEquipment(newItem, true, false);
    }

    public ItemUI GetHandItem() => handSlot.CheckIfItHasItem() == true ? handSlot.GetItemInSlot() : null;

    public ItemUI GetBodyItem() => bodySlot.CheckIfItHasItem() == true ? bodySlot.GetItemInSlot() : null;

    public ItemUI GetHeadItem() => headSlot.CheckIfItHasItem() == true ? headSlot.GetItemInSlot() : null;

    public Storage BackpackStorage()    
    {
        if(GetBodyItem())
            if(GetBodyItem().GetComponent<Storage>())
                return GetBodyItem().GetComponent<Storage>();
        
        return null;

    }
}
