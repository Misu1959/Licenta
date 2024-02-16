using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Resource))]
public class ResourceInspector : BaseInspector
{
    private Resource resource;

    private void Start() => resource = GetComponent<Resource>();

    public void OnMouseOver()
    {
        if (!InteractionManager.CanPlayerInteractWithWorld(false)) return;

        if (!resource.CheckIfCanBeGathered())
        {
            PopUpManager.instance.ShowMousePopUp("Wheel - inspect", PopUpManager.PopUpPriorityLevel.low);
            return;
        }

        PopUpManager.instance.ShowMousePopUp(hoverText, PopUpManager.PopUpPriorityLevel.low);
    }
}
