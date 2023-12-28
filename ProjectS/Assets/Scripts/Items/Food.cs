using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class Food : Item
{
    public float hungerAmount;
    public float hpAmount;

    public bool isCooking { get; private set;}

    private float maxTimeToCook = 1;
    private float timeToCook;

    private void Update()
    {
        Cook();
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        if (Input.GetMouseButtonDown(1))
            PlayerActionManagement.instance.SetTargetAndAction(this.gameObject, PlayerActionManagement.Action.eat);
    }

    public override void OnMouseEnter()
    {
        string popUpText = "LMB - Pick\nRMB - Eat";
        PopUpManager.instance.ShowMousePopUp(popUpText);
    }

    void Cook()
    {
        if (!isCooking)
            return;

        if (!PlayerActionManagement.instance.isPerformingAction)
        {
            SetIsCooking(false);
            return;
        }


        timeToCook -= Time.deltaTime;
        if (timeToCook<=0)
        {

            GetComponent<Item>().TakeFromStack(1);
            
            GameObject cookedItem = Instantiate(ItemsManager.instance.SearchItemsList(GetComponent<Item>().type + "C"));
            cookedItem.GetComponent<Item>().SetType(GetComponent<Item>().type + "C");
            cookedItem.GetComponent<Item>().AddToStack(1);
            InventoryManager.instance.AddItemToSlot(cookedItem);
            

            PlayerActionManagement.instance.CompleteAction();
            SetIsCooking(false);
            
        }
    }

    public void SetIsCooking(bool _isCooking)
    {
        isCooking = _isCooking;
        timeToCook = maxTimeToCook;

    }

    public void Consume()
    {
        GetComponent<Item>().TakeFromStack(1);
        PlayerStats.instance.Eat(hungerAmount, hpAmount);
    }
}
