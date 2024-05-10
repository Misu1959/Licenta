using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Item))]
public class ItemInspector : BaseInspector
{
    private Item item;

    private void Start() => item = GetComponent<Item>();
    
    public void OnMouseOver()
    {
        if (!InteractionManager.instance.CanPlayerInteractWithWorld(false) || !item.IsOnTheGround()) return;

        if (EquipmentManager.instance.BackpackStorage() && GetComponent<Storage>())
            PopUpManager.instance.ShowMousePopUp("LMB|RMB\nChange backpack\nWheel - inspect", PopUpManager.PopUpPriorityLevel.low);
        else if (GetComponent<Storage>())
            PopUpManager.instance.ShowMousePopUp("LMB|RMB\nEquip backpack\nWheel - inspect", PopUpManager.PopUpPriorityLevel.low);
        else 
            PopUpManager.instance.ShowMousePopUp(hoverText, PopUpManager.PopUpPriorityLevel.low);
    }
}
