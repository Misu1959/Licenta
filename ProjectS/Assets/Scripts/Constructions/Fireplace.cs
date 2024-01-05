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
        transform.GetChild(0).gameObject.SetActive(true);
    }

    public override void Update()
    {
        base.Update(); // it does fire stuff
        Cook();

    }

    private void OnMouseOver()
    {
        if (InventoryManager.instance.selectedItem)
        {
            if (InventoryManager.instance.selectedItem.GetComponent<Food>())
                PopUpManager.instance.ShowMousePopUp("Lmb - cook\nRMB - cancel", 1);
            else
                PopUpManager.instance.ShowMousePopUp("Lmb - add fuel\nRMB - cancel", 1);
        }

    }

    private void OnMouseExit()
    {
        PopUpManager.instance.ShowMousePopUp();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!InteractionManager.canInteract)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            if (InventoryManager.instance.selectedItem)
            {
                if (InventoryManager.instance.selectedItem.GetComponent<Food>())
                    PlayerActionManagement.instance.SetTargetAndAction(this.gameObject, PlayerActionManagement.Action.cook);
                else
                    PlayerActionManagement.instance.SetTargetAndAction(this.gameObject, PlayerActionManagement.Action.addFuel);
            }
        }

    }

    public void AddFuel()
    {
        if (InventoryManager.instance.selectedItem.fuelValue != 0)
        {
            timer.AddTime(InventoryManager.instance.selectedItem.fuelValue);
            InventoryManager.instance.selectedItem.TakeFromStack(1);
        }


    }

    public void Cook()
    {
        
        if (!PlayerActionManagement.instance.IsCooking(this.gameObject))
            return;

        InventoryManager.instance.selectedItem.GetComponent<Food>().timer.StartTimer();
        InventoryManager.instance.selectedItem.GetComponent<Food>().timer.Tick();


        if (!InventoryManager.instance.selectedItem.GetComponent<Food>().timer.IsElapsed())
            return;

        InventoryManager.instance.selectedItem.TakeFromStack(1);

        Item cookedItem = Instantiate(ItemsManager.instance.SearchItemsList(InventoryManager.instance.selectedItem.type + "C")).GetComponent<Item>();
        cookedItem.SetType(InventoryManager.instance.selectedItem.type + "C");
        cookedItem.AddToStack(1);
        InventoryManager.instance.AddItemToSlot(cookedItem);

        PlayerActionManagement.instance.CompleteAction();
    }

}
