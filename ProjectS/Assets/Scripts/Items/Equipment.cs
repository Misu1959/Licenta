using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Equipment : Item
{
    [SerializeField]
    private EquipmentData data;
    public override ItemData GetItemData() { return data; }

    public override void SetItemData(ItemData newData) { data = new EquipmentData((EquipmentData)newData); }

    public override void OnLeftMouseButtonPressed(int amount = -1)
    {
        PlayerActionManagement.instance.SetTargetAndAction(this.gameObject, PlayerActionManagement.Action.pick);
    }
    public override void OnRightMouseButtonPressed()
    {
        PlayerActionManagement.instance.SetTargetAndAction(this.gameObject, PlayerActionManagement.Action.equip);
    }

    public override void OnMouseOver()
    {
        if (!InteractionManager.CanPlayerInteractWithWorld(false)) return;
        if (!IsOnTheGround()) return;
        
        if(EquipmentManager.instance.BackpackStorage() && GetComponent<Storage>())
            PopUpManager.instance.ShowMousePopUp("LMB|RMB\nChange backpack");
        else if(GetComponent<Storage>())
            PopUpManager.instance.ShowMousePopUp("LMB|RMB\nEquip backpack");
        else
            PopUpManager.instance.ShowMousePopUp("LMB - Pick\nRMB - Equip");
    }

}
