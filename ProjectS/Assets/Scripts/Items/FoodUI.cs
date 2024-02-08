using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class FoodUI : ItemUI
{
    [SerializeField]
    private FoodData data;
    public override ItemData GetItemData() { return data; }

    public override void SetItemData(ItemData newData) { data = new FoodData((FoodData)newData); }

    public override void OnRightMouseButtonPressed() { Eat(); }

    private void Eat() {   PlayerStats.instance.Eat(GetComponent<Item_Base>());    }
    
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
