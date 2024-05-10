using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Chest : Construction, IPointerDownHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        if (!InteractionManager.instance.CanPlayerInteractWithWorld(false)) return;

        if (Input.GetMouseButtonDown(0))
            PlayerBehaviour.instance.SetTargetAndAction(this.transform, PlayerBehaviour.Action.search);
    }

}
