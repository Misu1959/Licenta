using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class EquipmentUI : ItemUI
{
    [SerializeField]
    private EquipmentData data;
    public override ItemData GetItemData() { return data; }

    public override void SetItemData(ItemData newData) { data = new EquipmentData((EquipmentData)newData); }

    public override void OnRightMouseButtonPressed() {   Equip();    }

    private void Equip() { EquipmentManager.instance.SetEquipment(this, true, false);   }

    public override void DisplayItem()
    {
        GetComponent<Image>().sprite = GetItemData().uiImg;

        if (InventoryManager.instance.selectedItemSlot.transform == transform.parent)
        {
            transform.GetChild(0).gameObject.SetActive(false);
            transform.GetChild(1).gameObject.SetActive(false);
        }
        else
        {
            transform.GetChild(0).gameObject.SetActive(true);
            transform.GetChild(1).gameObject.SetActive(true);

            transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = GetItemData().name.ToString();
            transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = GetEquipmentData().durability.ToString();
        }
    }

    public void UseTool()
    {
        data.durability--;
        if (data.durability <= 0)
        {
            InventoryManager.instance.RemoveItemFromSlot(this);
            EquipmentManager.instance.ReplenishItem(data.name);
            Destroy(this.gameObject);
        }
        DisplayItem();
    }
}
