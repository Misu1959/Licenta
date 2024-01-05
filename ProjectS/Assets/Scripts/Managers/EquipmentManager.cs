using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System.Diagnostics.CodeAnalysis;

public class EquipmentManager : MonoBehaviour
{
    public static EquipmentManager instance;

    [SerializeField] private GameObject equipment;

    private InventorySlot handSlot;
    private InventorySlot bodySlot;
    private InventorySlot headSlot;

    private void Start()
    {
        instance = this;
        SetSlots();
    }

    private void SetSlots()
    {
        handSlot = equipment.transform.GetChild(0).GetComponent<InventorySlot>();
        handSlot.GetComponent<Button>().onClick.AddListener(() => EquipSelectedItem(handSlot.transform));
        bodySlot = equipment.transform.GetChild(1).GetComponent<InventorySlot>();
        bodySlot.GetComponent<Button>().onClick.AddListener(() => EquipSelectedItem(bodySlot.transform));
        headSlot = equipment.transform.GetChild(2).GetComponent<InventorySlot>();
        headSlot.GetComponent<Button>().onClick.AddListener(() => EquipSelectedItem(headSlot.transform));
    }
    private void EquipSelectedItem(Transform slot)
    {
        if (slot.childCount > 0)
            return;

        if (!InventoryManager.instance.selectedItem)
            return;

        if (!InventoryManager.instance.selectedItem.GetComponent<Equipment>())
            return;

        //if (InventoryManager.instance.selectedItem.GetComponent<Equipment>().equipmentType != slot.GetSiblingIndex())
          //  return;

        InventoryManager.instance.selectedItem.transform.SetParent(slot);
        InventoryManager.instance.selectedItem.transform.localPosition = Vector2.zero;
        InventoryManager.instance.selectedItem = null;
    }

    public void UnequipHandItem(GameObject item)
    {
        if (handSlot.transform.childCount == 0)
            return;

        if (item != handSlot.transform.GetChild(0).gameObject)
            return;

        if (PlayerActionManagement.instance.currentAction >= PlayerActionManagement.Action.chop &&
            PlayerActionManagement.instance.currentAction <= PlayerActionManagement.Action.mine)
            PlayerActionManagement.instance.CancelAction();

    }

    public void SetEquipment(Equipment newEquipment)
    {
        if (!newEquipment)
            return;

        switch(newEquipment.equipmentType)
        {
            case Equipment.Type.hand:
                {
                    if (handSlot.transform.childCount > 0)
                    {
                        UnequipHandItem(newEquipment.gameObject);
                        InventoryManager.instance.AddItemToSlot(handSlot.GetItemInSlot());
                    }

                    newEquipment.transform.SetParent(handSlot.transform);
                    newEquipment.transform.localPosition = Vector2.zero;
                    break;
                }
            case Equipment.Type.body:
                {
                    InventoryManager.instance.AddItemToSlot(bodySlot.GetItemInSlot());

                    newEquipment.transform.SetParent(bodySlot.transform);
                    newEquipment.transform.localPosition = Vector2.zero;
                    break;
                }
            case Equipment.Type.head:
                {
                    InventoryManager.instance.AddItemToSlot(headSlot.GetItemInSlot());

                    newEquipment.transform.SetParent(headSlot.transform);
                    newEquipment.transform.localPosition = Vector2.zero;
                    break;
                }

        }
    }

    public Equipment GetHandItem()
    {
        if (handSlot.CheckIfItHasItem())
            return handSlot.GetItemInSlot().GetComponent<Equipment>();
        return null;
    }

    public Equipment GetBodyItem()
    {
        if (bodySlot.CheckIfItHasItem())
            return bodySlot.GetItemInSlot().GetComponent<Equipment>();
        return null;
    }

    public Equipment GetHeadItem()
    {
        if (headSlot.CheckIfItHasItem())
            return headSlot.GetItemInSlot().GetComponent<Equipment>();
        return null;
    }

}
