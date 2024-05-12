using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Equipment : Item
{
    [SerializeField]
    private EquipmentData data;
    public override ItemData GetItemData() => data;

    public override void SetItemData(ItemData newData) => data = new EquipmentData((EquipmentData)newData);

    public override void OnLeftMouseButtonPressed(int amount = -1) => PlayerBehaviour.instance.SetTargetAndAction(this.transform, PlayerBehaviour.Action.pick);
    
    public override void OnRightMouseButtonPressed() => PlayerBehaviour.instance.SetTargetAndAction(this.transform, PlayerBehaviour.Action.equip);


}
