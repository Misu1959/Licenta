using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class Item : Item_Base, IPointerDownHandler
{
    private SpriteRenderer spriteRenderer;

    void Awake() =>  spriteRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>(); 

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!InteractionManager.CanPlayerInteractWithWorld(false)) return;
        if (!IsOnTheGround()) return;

        if (Input.GetMouseButtonDown(0))
            OnLeftMouseButtonPressed();
        if (Input.GetMouseButtonDown(1))
            OnRightMouseButtonPressed();
    }
    public virtual void OnLeftMouseButtonPressed(int amount = -1) { Pick(); }

    private void Pick() {   PlayerActionManagement.instance.SetTargetAndAction(this.gameObject, PlayerActionManagement.Action.pick);    }

    public bool IsOnTheGround()
    {
        // If player is not droping it now return true
        if (PlayerActionManagement.instance.currentTarget == this.gameObject &&
        PlayerActionManagement.instance.currentAction == PlayerActionManagement.Action.drop)
            return false;
        else
            return true;
    }

    public void SetTransparent(bool setTransparent)
    {
        Color thisColor = spriteRenderer.color;

        if (setTransparent)
            spriteRenderer.color = new Color(thisColor.r, thisColor.g, thisColor.b, .5f);
        else
            spriteRenderer.color = new Color(thisColor.r, thisColor.g, thisColor.b, 1f);
    }
}
