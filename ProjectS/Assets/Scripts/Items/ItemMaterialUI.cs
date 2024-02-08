using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Net;
using System.Linq.Expressions;

public class ItemMaterialUI : ItemUI
{
    [SerializeField]
    private ItemData data;
    public override ItemData GetItemData() { return data; }

    public override void SetItemData(ItemData newData) { data = new ItemData(newData); }

    public override void OnRightMouseButtonPressed() {}

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
            transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = GetItemData().currentStack + " " + GetItemData().maxStack;
        }

    }

}
