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
            PlayerGatherManager.instance.SetTarget(this.gameObject, 33);
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

        if(timeToCook<=0)
        {
            GetComponent<ItemUI>().TakeFromStack(1);

            GameObject cookedItem = Instantiate(ItemsManager.instance.SearchItemsList(GetComponent<ItemUI>().type + "C"));

            cookedItem.GetComponent<Item>().SetType(GetComponent<ItemUI>().type + "C");
            cookedItem.GetComponent<Item>().AddToStack(1);
            
            InventoryManager.instance.AddItemToSlot(cookedItem);

            PlayerGatherManager.instance.SetTarget(null, 0);
            isCooking = false;
        }
        else
            timeToCook -= Time.deltaTime;
    }

    public void SetIsCooking(bool _isCooking)
    {
        isCooking = _isCooking;
        timeToCook = maxTimeToCook;

    }

    public void Consume()
    {
        if (hungerAmount == 0 && hpAmount == 0)
            return;

        GetComponent<Item>().TakeFromStack(1);

        PlayerStats.instance.Eat(GetComponent<Food>().hungerAmount, GetComponent<Food>().hpAmount);
    }
}
