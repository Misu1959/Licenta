using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Fireplace : Fire, IPointerDownHandler
{
    public bool isCampfire;
    public override void Start()
    {
        base.Start(); // it does fire stuff
    }

    public override void Update()
    {
        base.Update(); // it does fire stuff
        Cook();

    }

    private void OnMouseOver()
    {
        if (!InteractionManager.CanPlayerInteractWithWorld(true)) return;

        if (InventoryManager.instance.selectedItemSlot.GetItemInSlot()?.GetComponent<Food>())
            PopUpManager.instance.ShowMousePopUp("Lmb - cook\nRMB - cancel", PopUpManager.PopUpPriorityLevel.low);
        else
            PopUpManager.instance.ShowMousePopUp("Lmb - add fuel\nRMB - cancel", PopUpManager.PopUpPriorityLevel.low);

    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!InteractionManager.CanPlayerInteractWithWorld(true)) return;

        if (Input.GetMouseButtonDown(0))
        {
            if (InventoryManager.instance.selectedItemSlot.GetItemInSlot()?.GetComponent<Food>())
                PlayerBehaviour.instance.SetTargetAndAction(this.transform, PlayerBehaviour.Action.cook);
            else
                PlayerBehaviour.instance.SetTargetAndAction(this.transform, PlayerBehaviour.Action.addFuel);
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

        InventoryManager.instance.selectedItemSlot.GetItemInSlot().TakeFromStack(1);

        Item_Base cookedItem = ItemsManager.instance.CreateItem(InventoryManager.instance.selectedItemSlot.GetItemInSlot().GetItemData().name + 1);
        InventoryManager.instance.AddItemToInventory(cookedItem);

        PlayerBehaviour.instance.CompleteAction();
    }

}
