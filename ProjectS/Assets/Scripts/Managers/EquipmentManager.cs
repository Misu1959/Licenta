using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System.Diagnostics.CodeAnalysis;

public class EquipmentManager : MonoBehaviour
{
    public static EquipmentManager instance;

    [SerializeField] private GameObject equipment;

    private GameObject handSlot;
    private GameObject bodySlot;
    private GameObject headSlot;

    private void Start()
    {
        instance = this;
        SetSlots();
    }

    private void SetSlots()
    {
        handSlot = equipment.transform.GetChild(0).gameObject;
        handSlot.GetComponent<Button>().onClick.AddListener(() => EquipSelectedItem(handSlot.transform));
        bodySlot = equipment.transform.GetChild(1).gameObject;
        bodySlot.GetComponent<Button>().onClick.AddListener(() => EquipSelectedItem(bodySlot.transform));
        headSlot = equipment.transform.GetChild(2).gameObject;
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

        if (InventoryManager.instance.selectedItem.GetComponent<Equipment>().equipmentNumber != slot.GetSiblingIndex())
            return;

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

        PlayerGatherManager.instance.CancelAction();

    }

    public void SetEquipment(Equipment newEquipment)
    {
        if (!newEquipment)
            return;

        if (newEquipment.equipmentNumber == 0)
        {
            if (handSlot.transform.childCount > 0)
            {
                UnequipHandItem(newEquipment.gameObject);
                InventoryManager.instance.AddItemToSlot(handSlot.transform.GetChild(0).gameObject);
            }
            newEquipment.transform.SetParent(handSlot.transform);
            newEquipment.transform.localPosition = Vector2.zero;
        }
        else if (newEquipment.equipmentNumber == 1)
        {
            if (bodySlot.transform.childCount > 0)
                InventoryManager.instance.AddItemToSlot(bodySlot.transform.GetChild(0).gameObject);

            newEquipment.transform.SetParent(bodySlot.transform);
            newEquipment.transform.localPosition = Vector2.zero;
        }
        else if (newEquipment.equipmentNumber == 2)
        {
            if (headSlot.transform.childCount > 0)
                InventoryManager.instance.AddItemToSlot(headSlot.transform.GetChild(0).gameObject);

            newEquipment.transform.SetParent(headSlot.transform);
            newEquipment.transform.localPosition = Vector2.zero;
        }

    }

    public bool CheckWhatItemIsEquipedInHand(int req)
    {
        if (handSlot.transform.childCount == 0)
            return false;

        if (handSlot.transform.GetChild(0).GetComponent<Equipment>().actionNumber != req)
            return false;

        return true;
    }

    public Equipment GetHandItem()
    {
        if (handSlot.transform.childCount > 0)
            return handSlot.transform.GetChild(0).GetComponent<Equipment>();
        return null;
    }

    public Equipment GetBodyItem()
    {
        if (bodySlot.transform.childCount > 0)
            return bodySlot.transform.GetChild(0).GetComponent<Equipment>();
        return null;
    }

    public Equipment GetHeadItem()
    {
        if (headSlot.transform.childCount > 0)
            return headSlot.transform.GetChild(0).GetComponent<Equipment>();
        return null;
    }

}
