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
            if (IsOnTheGround())
                PlayerActionManagement.instance.SetTargetAndAction(this.gameObject, PlayerActionManagement.Action.eat);
    }

    public override void OnMouseEnter()
    {
        if (IsOnTheGround())
        { 
            string popUpText = "LMB - Pick\nRMB - Eat";
            PopUpManager.instance.ShowMousePopUp(popUpText);
        }
    }

    void Cook()
    {
        if (!IsBeingCooked())
        {
            timeToCook = maxTimeToCook;
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
        }
    }

    public void Consume()
    {
        GetComponent<Item>().TakeFromStack(1);
        PlayerStats.instance.Eat(hungerAmount, hpAmount);
    }

    protected bool IsBeingCooked()
    {
        if (PlayerActionManagement.instance.currentTarget == this.gameObject &&
            PlayerActionManagement.instance.currentAction == PlayerActionManagement.Action.cook &&
            PlayerActionManagement.instance.isPerformingAction)
            return true;
        else
            return false;
    }
}
