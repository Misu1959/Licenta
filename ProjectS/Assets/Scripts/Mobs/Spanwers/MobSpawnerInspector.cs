using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ComplexMobSpawner))]
public class MobSpawnerInspector : BaseInspector
{
    public void OnMouseOver()
    {
        if (!InteractionManager.instance.CanPlayerInteractWithWorld(false)) return;

        if (CanPlayerInteractWithSpawner())
            PopUpManager.instance.ShowMousePopUp(hoverText, PopUpManager.PopUpPriorityLevel.low);
        else
            PopUpManager.instance.ShowMousePopUp("Wheel - inspect", PopUpManager.PopUpPriorityLevel.low);
    }


    private bool CanPlayerInteractWithSpawner() => false;

}
