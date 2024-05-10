using UnityEngine;

public class ItemMaterial : Item
{
    [SerializeField]
    private ItemData data;
    public override ItemData GetItemData() => data;
    public override void SetItemData(ItemData newData) => data = new ItemData(newData);

    public override void OnRightMouseButtonPressed(){}



    public void LoadData(int _stack) => data.currentStack = _stack;


}