using UnityEngine;

public class ItemMaterial : Item
{
    [SerializeField]
    private ItemData data;
    public override ItemData GetItemData() {   return data;    }

    public override void SetItemData(ItemData newData) { data = new ItemData(newData); }

    public override void OnRightMouseButtonPressed(){}

}