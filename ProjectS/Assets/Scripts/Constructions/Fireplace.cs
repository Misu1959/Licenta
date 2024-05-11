using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Fireplace : Fire, IPointerDownHandler
{
    public override void Update()
    {
        base.Update(); // it does fire stuff
        Cook();

    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!InteractionManager.instance.CanPlayerInteractWithWorld(true)) return;

        if (Input.GetMouseButtonDown(0))
        {
            ItemUI selectedItem = InventoryManager.instance.selectedItemSlot.GetItemInSlot();
            if (selectedItem)
            {
                if (IsFireOn() &&
                   selectedItem.GetComponent<FoodUI>() &&
                   selectedItem.GetFoodData().canBeCoocked)
                    PlayerBehaviour.instance.SetTargetAndAction(this.transform, PlayerBehaviour.Action.cook);
                else if(selectedItem.GetItemData().fuelValue > 0)
                    PlayerBehaviour.instance.SetTargetAndAction(this.transform, PlayerBehaviour.Action.addFuel);
            }
        }
    }

    public void AddFuel()
    {
        if (InventoryManager.instance.selectedItemSlot.GetItemInSlot().GetItemData().fuelValue != 0)
        {
            fireTimer.AddTime(InventoryManager.instance.selectedItemSlot.GetItemInSlot().GetItemData().fuelValue);
            InventoryManager.instance.selectedItemSlot.GetItemInSlot().TakeFromStack(1);
        }


    }

    public void Cook()
    {
        
        if (!PlayerBehaviour.instance.IsCooking(this.transform)) return;

        InventoryManager.instance.selectedItemSlot.GetItemInSlot().GetFoodData().cookTimer.StartTimer();
        InventoryManager.instance.selectedItemSlot.GetItemInSlot().GetFoodData().cookTimer.Tick();


        if (!InventoryManager.instance.selectedItemSlot.GetItemInSlot().GetFoodData().cookTimer.IsElapsed()) return;

        Item_Base cookedItem = ItemsManager.instance.CreateItem(InventoryManager.instance.selectedItemSlot.GetItemInSlot().GetItemData().objectName + 1);
        InventoryManager.instance.AddItemToInventory(cookedItem);

        InventoryManager.instance.selectedItemSlot.GetItemInSlot().TakeFromStack(1);
        PlayerBehaviour.instance.CompleteAction();
    }

}
