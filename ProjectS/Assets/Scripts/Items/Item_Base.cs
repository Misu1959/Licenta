using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class Item_Base : MonoBehaviour
{
    public abstract ItemData GetItemData();
    public FoodData GetFoodData() => (FoodData)GetItemData();
    public EquipmentData GetEquipmentData() => (EquipmentData)GetItemData();

    public abstract void SetItemData(ItemData newData);

    public abstract void OnRightMouseButtonPressed();


    public bool CheckIfStackIsFull()
    {
        if (GetItemData().currentStack == GetItemData().maxStack)
            return true;

        return false;
    }

    public virtual void AddToStack(int _amountToAdd)
    {
        GetItemData().currentStack += _amountToAdd;
    }

    public virtual void TakeFromStack(int _amountToTake)
    {
        GetItemData().currentStack -= _amountToTake;

        if (GetItemData().currentStack <= 0)
            Destroy(this.gameObject);

    }

}
