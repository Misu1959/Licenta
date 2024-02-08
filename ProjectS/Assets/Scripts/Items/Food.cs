using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class Food : Item
{
    [SerializeField]
    private FoodData data;
    public override ItemData GetItemData() { return data; }

    public override void SetItemData(ItemData newData) { data = new FoodData((FoodData)newData); }

    public override void OnRightMouseButtonPressed() 
    {
        PlayerActionManagement.instance.SetTargetAndAction(this.gameObject, PlayerActionManagement.Action.eat); 
    }

    public override void OnMouseOver()
    {
            
        if (!InteractionManager.CanPlayerInteractWithWorld(false)) return;

        if (IsOnTheGround())
        { 
            string popUpText = "LMB - Pick\nRMB - Eat";
            PopUpManager.instance.ShowMousePopUp(popUpText);
        }
    }
}
