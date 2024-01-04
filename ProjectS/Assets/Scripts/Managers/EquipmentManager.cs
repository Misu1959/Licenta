using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System.Diagnostics.CodeAnalysis;

public class EquipmentManager : MonoBehaviour
{
    public static EquipmentManager instance;

    [SerializeField] private GameObject equipment;

    private Transform handSlot;
    private Transform bodySlot;
    private Transform headSlot;

    private void Start()
    {
        instance = this;
        SetSlots();
    }

    private void SetSlots()
    {
        handSlot = equipment.transform.GetChild(0);
        handSlot.GetComponent<Button>().onClick.AddListener(() => EquipSelectedItem(handSlot));
        bodySlot = equipment.transform.GetChild(1);
        bodySlot.GetComponent<Button>().onClick.AddListener(() => EquipSelectedItem(bodySlot));
        headSlot = equipment.transform.GetChild(2);
        headSlot.GetComponent<Button>().onClick.AddListener(() => EquipSelectedItem(headSlot));
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
        InventoryManager.instance.selectedItem.GetComponent<Image>().raycastTarget = true;
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
                        InventoryManager.instance.AddItemToSlot(handSlot.transform.GetChild(0).gameObject);
                    }

                    newEquipment.transform.SetParent(handSlot);
                    newEquipment.transform.localPosition = Vector2.zero;
                    break;
                }
            case Equipment.Type.body:
                {
                    InventoryManager.instance.AddItemToSlot(bodySlot.transform.GetChild(0).gameObject);

                    newEquipment.transform.SetParent(bodySlot);
                    newEquipment.transform.localPosition = Vector2.zero;
                    break;
                }
            case Equipment.Type.head:
                {
                    InventoryManager.instance.AddItemToSlot(headSlot.transform.GetChild(0).gameObject);

                    newEquipment.transform.SetParent(headSlot);
                    newEquipment.transform.localPosition = Vector2.zero;
                    break;
                }

        }
    }

    public Equipment GetHandItem()
    {
        if (handSlot.childCount > 0)
            return handSlot.GetChild(0).GetComponent<Equipment>();
        return null;
    }

    public Equipment GetBodyItem()
    {
        if (bodySlot.childCount > 0)
            return bodySlot.GetChild(0).GetComponent<Equipment>();
        return null;
    }

    public Equipment GetHeadItem()
    {
        if (headSlot.childCount > 0)
            return headSlot.GetChild(0).GetComponent<Equipment>();
        return null;
    }

}
