using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Fireplace : Fire, IPointerDownHandler
{
    public bool isCampfire;
    void Start()
    {
        transform.GetChild(0).gameObject.SetActive(true);
    }

    void Update()
    {
        Cook();
    }
    private void OnMouseEnter()
    {


    }

    private void OnMouseExit()
    {

    }
    public void OnPointerDown(PointerEventData eventData)
    {
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
        /*
        if (!PlayerActionManagement.instance.IsCooking(this.gameObject))
        {
            timeToCook = maxTimeToCook;
            return;
        }

        timeToCook -= Time.deltaTime;
        if (timeToCook <= 0)
        {

            GetComponent<Item>().TakeFromStack(1);

            GameObject cookedItem = Instantiate(ItemsManager.instance.SearchItemsList(GetComponent<Item>().type + "C"));
            cookedItem.GetComponent<Item>().SetType(GetComponent<Item>().type + "C");
            cookedItem.GetComponent<Item>().AddToStack(1);
            InventoryManager.instance.AddItemToSlot(cookedItem);


            PlayerActionManagement.instance.CompleteAction();
        }*/
    }

}
