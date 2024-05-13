using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Construction))]
public class ConstructionInspector : BaseInspector
{
    [SerializeField] private bool doPlayerNeedSelectedItemToInteract;

    public void OnMouseOver()
    {
        if (!InteractionManager.instance.CanPlayerInteractWithWorld(doPlayerNeedSelectedItemToInteract)) return;


        if(CanPlayerInteractWithConstruction())
            PopUpManager.instance.ShowMousePopUp(hoverText, PopUpManager.PopUpPriorityLevel.high);
        else
            PopUpManager.instance.ShowMousePopUp("Wheel - inspect", PopUpManager.PopUpPriorityLevel.low);

    }

    private bool CanPlayerInteractWithConstruction()
    {
        if (!doPlayerNeedSelectedItemToInteract)
            return true;
        else if (!InventoryManager.instance.selectedItemSlot.GetItemInSlot())
            return false;
        else
        {
            ItemUI selectedItem = InventoryManager.instance.selectedItemSlot.GetItemInSlot();
            if (GetComponent<Fireplace>())
            {
                if (GetComponent<Fireplace>().IsFireOn() &&
                    selectedItem.GetComponent<FoodUI>() &&
                    selectedItem.GetFoodData().canBeCooked)
                {
                    hoverText = "Lmb - cook\nWheel - inspect";
                    return true;
                }
                else if (selectedItem.GetItemData().fuelValue > 0)
                {
                    hoverText = "Lmb - add fuel\nWheel - inspect";
                    return true;
                }
            }
            return false;
        }
    }
}
