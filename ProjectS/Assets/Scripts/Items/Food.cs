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

    public void Consume()
    {
        GetComponent<Item>().TakeFromStack(1);
        PlayerStats.instance.Eat(hungerAmount, hpAmount);
    }


}
