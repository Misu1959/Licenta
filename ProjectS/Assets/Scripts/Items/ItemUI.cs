using UnityEngine.UI;

public abstract class ItemUI: Item_Base
{

    public override void AddToStack(int _amountToAdd)
    {
        base.AddToStack(_amountToAdd);
        DisplayItem();
        StartCoroutine(CraftingManager.instance.RefreshCraftingManager());
    }

    public override void TakeFromStack(int _amountToTake)
    {
        base.TakeFromStack(_amountToTake);

        if (GetItemData().currentStack <= 0)
            InventoryManager.instance.RemoveItemFromSlot(this);

        DisplayItem();
        StartCoroutine(CraftingManager.instance.RefreshCraftingManager());

    }

    public abstract void DisplayItem();
}
