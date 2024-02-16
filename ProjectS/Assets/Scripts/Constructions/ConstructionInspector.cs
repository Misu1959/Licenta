using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Construction))]
public class ConstructionInspector : BaseInspector
{
    private Construction construction;

    private void Start() => construction = GetComponent<Construction>();

    public void OnMouseOver()
    {
        if (!InteractionManager.CanPlayerInteractWithWorld(false)) return;


        PopUpManager.instance.ShowMousePopUp(hoverText, PopUpManager.PopUpPriorityLevel.low);
    }
}
