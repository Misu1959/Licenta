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
    public virtual void OnLeftMouseButtonPressed(int amount = -1) => Pick(); 
    private void Pick() =>   PlayerBehaviour.instance.SetTargetAndAction(this.transform, PlayerBehaviour.Action.pick); 

    public bool IsOnTheGround()
    {
        // If player is not droping it now return true
        if (PlayerBehaviour.instance.currentTarget == this.gameObject &&
        PlayerBehaviour.instance.currentAction == PlayerBehaviour.Action.drop)
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
