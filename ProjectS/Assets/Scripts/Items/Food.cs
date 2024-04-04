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
        PlayerBehaviour.instance.SetTargetAndAction(this.transform, PlayerBehaviour.Action.eat); 
    }

}
