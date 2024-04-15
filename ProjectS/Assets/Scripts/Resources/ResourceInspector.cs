using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Resource))]
public class ResourceInspector : BaseInspector
{
    public void OnMouseOver()
    {
        if (!InteractionManager.CanPlayerInteractWithWorld(false)) return;

        if (!GetComponent<Resource>().CheckIfCanBeGathered())
            PopUpManager.instance.ShowMousePopUp("Wheel - inspect", PopUpManager.PopUpPriorityLevel.low);
        else
            PopUpManager.instance.ShowMousePopUp(hoverText, PopUpManager.PopUpPriorityLevel.low);
    }
}
