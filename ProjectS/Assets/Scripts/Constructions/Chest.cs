using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Chest : Construction, IPointerDownHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        if (!InteractionManager.CanPlayerInteractWithWorld(false)) return;

        if (Input.GetMouseButtonDown(0))
            PlayerActionManagement.instance.SetTargetAndAction(this.gameObject, PlayerActionManagement.Action.search);
    }

    public void OnMouseOver()
    {
        if (!InteractionManager.CanPlayerInteractWithWorld(false)) return;

        PopUpManager.instance.ShowMousePopUp("Lmb - search", PopUpManager.PopUpPriorityLevel.low);

    }

    public void OnMouseExit() { PopUpManager.instance.ShowMousePopUp(); }
}
